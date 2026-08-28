# READ BEFORE CODING

Don't EVER create .md files unless explicitly asked to do so by the user

This document defines the coding standards, architecture patterns, and design principles for the Giftastic e-commerce backend. Read this completely before making any code changes.

---

## Architecture: Modular Monolith

This is a single Spring Boot application organized into independent modules. Each module represents a bounded context in the domain.

### Module Structure

```
src/main/java/com/giftastic/giftastic/
├── common/              # Shared utilities, security, DTOs
│   ├── dto/
│   └── security/
└── modules/             # Business modules
    ├── admin/
    ├── cart/
    ├── identity/
    ├── order/
    ├── product/
    ├── user/
    └── vendor/
```

## What to avoid when developing

Avoid tight coupling, avoid complex or unnecessary logic
Everything is separated and loosely coupled

### What to avoid with DB

Don't write manual sql files to update DB, always use auto-update

### security

User is the archtype of this project, admins and vendors have an is-a relationship with users

a user can be a vendor and a super_admin

any action for any controller needs to be double checked that it has the correct authorization flag

there is a comprehensive security handling classes already implemented, rely on extending/implementing them rather than re-creating logic

### how to contribute to frontend

any frontend changes or additions need to be double checked that the communication and DTOs are being sent and represented correctly between frontend, backend and Database

these issues are only seen once thorough testing is conducted, so they can easily be avoided by looking at the controllers, DTOs, and permissions required, then making the design

### what must be done

before contributing or making changes, you must read ALL of the necessary or related code files, understand how they currently operate, what needs to be changed/added, why, and what is the simplest, (following the other rules mentioned in this document) and most efficient way to follow and implement the prompt in the said existing/new files

no code can or shall be made without understanding the context of everything, analyzing the best approaches to implement a solution, then implementing it

### what to avoid in general

avoid making changes that help current task be considered done, but lacking or causing technical debt

it is very crucial to treat this as a long-term first, rather than short-term first

it is very important to avoid making assumptions if a prompt made to you was not clear

### Layer Organization

Each module follows a consistent 4-layer structure:

```
module/
├── controller/    # REST endpoints, request/response handling
├── service/       # Business logic, orchestration
├── domain/        # Entities, value objects, enums
└── repository/    # Data access (Spring Data JPA)
```

**Why this structure?**
- Clear separation of concerns
- Easy to navigate and locate code
- Enforces dependency direction (controller → service → repository)
- Testable in isolation

---

## Module Boundaries & Dependencies

### Rules

1. **Keep logic in the right module** - Product logic goes in `product`, order logic in `order`, etc.

2. **No circular dependencies** - Module A can call Module B, but B cannot call A back.

3. **Minimize cross-module calls** - If you need data from another module, consider:
   - Passing IDs and letting the caller resolve them
   - Creating a shared DTO in `common/dto/`
   - Using domain events (future enhancement)

4. **Don't expose internals** - Only service interfaces should be called across modules, never repositories or domain entities directly.

### Example: Good vs Bad

```java
// ❌ BAD: Order module reaching into Product repository
@Service
class OrderService {
    @Autowired
    private ProductRepository productRepo; // WRONG - crosses module boundary
}

// ✅ GOOD: Order module calls Product service
@Service
class OrderService {
    @Autowired
    private ProductService productService; // Correct - uses public API
}
```

---

## Code Style: KISS (Keep It Simple, Stupid)

### Core Principles

1. **Minimal code** - Write only what's necessary to solve the problem
2. **No premature optimization** - Solve the problem first, optimize later if needed
3. **Prefer clarity over cleverness** - Simple code beats clever code
4. **Avoid over-engineering** - Don't add abstractions "just in case"

### Lombok Usage

We use Lombok extensively to reduce boilerplate. Required annotations:

**Entities:**
```java
@Entity
@Table(name = "products")
@Getter
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class Product {
    // fields
}
```

**Services:**
```java
@Service
@RequiredArgsConstructor  // Constructor injection for final fields
public class ProductService {
    private final ProductRepository repository;
}
```

**DTOs:**
```java
public record LoginRequest(String email, String password) {}
// Use records for immutable DTOs - no Lombok needed
```

**Why Lombok?**
- Eliminates 80% of boilerplate (getters, constructors, builders)
- Keeps classes focused on business logic
- Reduces maintenance burden
- IDE support is excellent

---

## Comments: Less is More

### When to Comment

**DO comment:**
- Complex business rules that aren't obvious from code
- Non-obvious security decisions
- Workarounds for external API quirks

**DON'T comment:**
- What the code does (code should be self-explanatory)
- Obvious getters/setters (Lombok handles this)
- Restating method names

### Examples

```java
// ❌ BAD: Obvious comment
// Get user by ID
public User getUserById(UUID id) {
    return repository.findById(id);
}

// ✅ GOOD: No comment needed - method name is clear
public User getUserById(UUID id) {
    return repository.findById(id);
}

// ✅ GOOD: Explains non-obvious business rule
public void submitForApproval() {
    // Products must be in DRAFT status to prevent re-submission of rejected items
    ensureStatus(ProductStatus.DRAFT);
    status = ProductStatus.PENDING_APPROVAL;
}
```

**Why minimal comments?**
- Comments rot - code changes, comments don't
- Self-documenting code is better than commented code
- Forces you to write clearer code
- Reduces visual noise

---

## Domain-Driven Design Patterns

### Entities

Entities are the core domain objects with identity and lifecycle.

**Pattern:**
```java
@Entity
@Table(name = "products")
@Getter
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class Product {
    @Id
    @NonNull
    private UUID id;
    
    @NonNull
    private String name;
    
    // Static factory method for creation
    public static Product create(String name, BigDecimal price) {
        validateName(name);
        validatePrice(price);
        return new Product(UUID.randomUUID(), name, price);
    }
    
    // Business methods that maintain invariants
    public void updatePrice(BigDecimal newPrice) {
        validatePrice(newPrice);
        this.price = newPrice;
        touch();
    }
    
    private static void validatePrice(BigDecimal price) {
        if (price.compareTo(BigDecimal.ZERO) <= 0) {
            throw new IllegalArgumentException("Price must be positive");
        }
    }
    
    private void touch() {
        this.updatedAt = LocalDateTime.now();
    }
}
```

**Why this pattern?**
- Factory methods enforce validation at creation
- Business methods keep entities in valid states
- Private constructors prevent invalid object creation
- Validation logic lives with the domain, not scattered in services

### Value Objects

Use `@Embeddable` for value objects that have no identity.

```java
@Embeddable
@Getter
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@AllArgsConstructor
public class CartItem {
    private UUID productId;
    private int quantity;
    
    public void increaseQuantity(int amount) {
        this.quantity += amount;
    }
}
```

### Enums for State

Always use enums for status fields, never strings.

```java
public enum ProductStatus {
    DRAFT,
    PENDING_APPROVAL,
    APPROVED,
    REJECTED,
    DISABLED
}
```

**Why enums?**
- Type safety - impossible to have invalid states
- IDE autocomplete
- Refactoring support
- Clear domain vocabulary

---

## Service Layer Patterns

### Keep Services Thin

Services orchestrate, they don't contain business logic. Business logic lives in domain entities.

```java
// ❌ BAD: Business logic in service
@Service
@RequiredArgsConstructor
public class ProductService {
    private final ProductRepository repository;
    
    public void updatePrice(UUID productId, BigDecimal newPrice) {
        Product product = repository.findById(productId)
            .orElseThrow(() -> new NotFoundException("Product not found"));
        
        // Business logic in service - WRONG
        if (newPrice.compareTo(BigDecimal.ZERO) <= 0) {
            throw new IllegalArgumentException("Price must be positive");
        }
        product.setPrice(newPrice);
        product.setUpdatedAt(LocalDateTime.now());
        
        repository.save(product);
    }
}

// ✅ GOOD: Service delegates to domain
@Service
@RequiredArgsConstructor
public class ProductService {
    private final ProductRepository repository;
    
    public void updatePrice(UUID productId, BigDecimal newPrice) {
        Product product = repository.findById(productId)
            .orElseThrow(() -> new NotFoundException("Product not found"));
        
        product.updatePrice(newPrice); // Domain handles validation and state
        repository.save(product);
    }
}
```

**Why thin services?**
- Domain logic is reusable and testable
- Services become simple orchestration
- Easier to understand and maintain
- Follows DDD principles

### Transaction Boundaries

Use `@Transactional` on service methods, not repositories.

```java
@Service
@RequiredArgsConstructor
public class OrderService {
    
    @Transactional
    public Order placeOrder(UUID customerId, List<OrderItem> items) {
        Order order = Order.place(customerId, items);
        return repository.save(order);
    }
}
```

**Why service-level transactions?**
- Service methods define business operations
- Multiple repository calls can be in one transaction
- Clear transaction boundaries

---

## Controller Layer Patterns

### REST Conventions

Follow standard HTTP semantics:

- `POST` - Create new resources
- `GET` - Retrieve resources
- `PATCH` - Partial updates
- `DELETE` - Remove resources
- `PUT` - Full replacement (rarely used)

### Response Codes

```java
@PostMapping
public ResponseEntity<Product> create(@RequestBody CreateProductRequest request) {
    Product product = productService.create(request);
    return ResponseEntity.status(201).body(product); // 201 Created
}

@PatchMapping("/{id}")
public ResponseEntity<Void> update(@PathVariable UUID id, @RequestBody UpdateRequest request) {
    productService.update(id, request);
    return ResponseEntity.noContent().build(); // 204 No Content
}

@GetMapping("/{id}")
public ResponseEntity<Product> get(@PathVariable UUID id) {
    return ResponseEntity.ok(productService.getById(id)); // 200 OK
}
```

### DTOs for Requests/Responses

Never expose entities directly in controllers. Use DTOs.

```java
// Use records for immutable DTOs
public record CreateProductRequest(
    String name,
    BigDecimal price,
    UUID categoryId,
    String description
) {}

// Controller
@PostMapping
public ResponseEntity<ProductResponse> create(@RequestBody CreateProductRequest request) {
    Product product = productService.create(request);
    return ResponseEntity.status(201).body(ProductResponse.from(product));
}
```

**Why DTOs?**
- Decouples API from domain model
- Prevents over-fetching (Jackson serialization issues)
- API versioning flexibility
- Clear API contracts

---

## Security Patterns

### Permission-Based Access Control

We use Spring Security with custom permission evaluation.

```java
@PatchMapping("/{productId}/approve")
@PreAuthorize("hasPermission(null, 'ACTIVATE_PRODUCTS')")
public ResponseEntity<Void> approve(@PathVariable UUID productId) {
    productService.approveProduct(productId);
    return ResponseEntity.noContent().build();
}
```

### Resource Ownership

For user-owned resources, pass the owner ID to the permission evaluator:

```java
@PostMapping("/{customerId}/items")
@PreAuthorize("hasPermission(#customerId, 'USER_OWNER')")
public ResponseEntity<Void> addItem(
    @PathVariable UUID customerId,
    @RequestParam UUID productId,
    @RequestParam int quantity
) {
    cartService.addItem(customerId, productId, quantity);
    return ResponseEntity.ok().build();
}
```

**How it works:**
- `DomainPermissionEvaluator` checks if the authenticated user has the permission
- For ownership checks, it compares `principal.getUserId()` with the resource owner ID
- Admins with `SUPER_ADMIN` bypass all checks

**Why this approach?**
- Declarative security at the endpoint level
- Centralized permission logic
- Easy to audit who can do what
- Flexible for both role-based and resource-based access

---

## Database Patterns

### Use UUIDs for IDs

All entities use `UUID` as primary keys, not auto-increment integers.

```java
@Id
@NonNull
private UUID id;
```

**Why UUIDs?**
- No coordination needed across services (future microservices)
- Prevents enumeration attacks
- Can generate IDs before persistence
- Distributed system friendly

### Eager vs Lazy Loading

Default to `LAZY` for collections, use `EAGER` only when always needed.

```java
// Default - lazy loading
@OneToMany(mappedBy = "order")
private List<OrderItem> items;

// Explicit eager when always accessed
@ElementCollection(fetch = FetchType.EAGER)
private List<CartItem> items;
```

**Why lazy by default?**
- Prevents N+1 query problems
- Loads only what's needed
- Better performance for large collections

### Embeddables for Value Objects

Use `@ElementCollection` for value objects that don't need separate tables.

```java
@ElementCollection(fetch = FetchType.EAGER)
@CollectionTable(name = "cart_items", joinColumns = @JoinColumn(name = "cart_id"))
private List<CartItem> items;
```

**Why embeddables?**
- Simpler than separate entities
- No unnecessary joins
- Value objects don't need identity

---

## Error Handling

### Domain Exceptions

Throw meaningful exceptions from domain logic:

```java
public void decreaseStock(int quantity) {
    if (quantity <= 0) {
        throw new IllegalArgumentException("Quantity must be positive");
    }
    if (stockQuantity - quantity < 0) {
        throw new IllegalStateException("Insufficient stock");
    }
    stockQuantity -= quantity;
}
```

### Service Exceptions

Services throw domain-specific exceptions:

```java
public Product getById(UUID id) {
    return repository.findById(id)
        .orElseThrow(() -> new ProductNotFoundException(id));
}
```

**Why specific exceptions?**
- Clear error semantics
- Easy to handle at controller level
- Better error messages for clients
- Supports future global exception handler

---

## Testing Philosophy

### What to Test

1. **Domain logic** - Unit test entity business methods
2. **Service orchestration** - Test service methods with mocked repositories
3. **Integration** - Test full request/response cycles

### What NOT to Test

1. Getters/setters (Lombok-generated)
2. Simple CRUD operations
3. Framework code (Spring, JPA)

**Why this approach?**
- Focus on business logic, not boilerplate
- Fast test execution
- High value tests only

---

## Design Decision Guidelines

When making design choices, follow this priority:

1. **Simplicity** - Choose the simpler solution
2. **Maintainability** - Code will be read 10x more than written
3. **Performance** - Optimize only when needed, measure first
4. **Flexibility** - Don't add flexibility you don't need today

### Common Decisions

**Q: Should I create an interface for this service?**
A: No, unless you have multiple implementations. YAGNI (You Aren't Gonna Need It).

**Q: Should I add caching?**
A: Not until you measure a performance problem. Premature optimization is evil.

**Q: Should I validate in controller or service?**
A: Domain entities validate their own state. Controllers validate request format (use `@Valid`).

**Q: Should I use DTOs or entities in responses?**
A: Always DTOs. Never expose entities directly.

**Q: Should I add pagination to this endpoint?**
A: Yes, if it returns a collection. Always paginate lists.

---

## Quick Reference Checklist

Before submitting code, verify:

- [ ] Code is in the correct module
- [ ] No circular dependencies between modules
- [ ] Business logic is in domain entities, not services
- [ ] Using Lombok annotations (`@Getter`, `@RequiredArgsConstructor`, etc.)
- [ ] Minimal or no comments (code is self-documenting)
- [ ] Using UUIDs for entity IDs
- [ ] DTOs for controller requests/responses
- [ ] `@PreAuthorize` on protected endpoints
- [ ] Enums for status fields
- [ ] Factory methods for entity creation
- [ ] Validation in domain entities
- [ ] `@Transactional` on service methods that modify data
- [ ] Proper HTTP status codes in responses

---

## Summary

This codebase values:
- **Simplicity** over complexity
- **Clarity** over cleverness
- **Domain-driven design** over anemic models
- **Minimal code** over comprehensive code
- **Self-documenting code** over commented code

When in doubt, choose the simpler, more obvious solution.
