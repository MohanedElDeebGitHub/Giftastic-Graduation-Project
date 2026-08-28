# Modular Monolith (How we organize code)

This codebase is a **modular monolith**: one deployable Spring Boot application, organized into clear modules.

## Modules

All business code lives under:

- `src/main/java/com/giftastic/giftastic/modules/*`

Current modules:

- `admin`
- `cart`
- `identity`
- `order`
- `product`
- `user`
- `vendor`

## Rules (keep it simple)

- **Put code where it belongs**: product logic goes in `product`, order logic goes in `order`, etc.
- **Don't leak internals**: avoid reaching into another module's internal classes unless it is intentionally part of its public surface.
- **No circular dependencies**: modules should not end up depending on each other in a loop.
- **Prefer clean boundaries**: when a module needs something from another module, use a clear service/contract rather than mixing responsibilities.

## Review expectation

If a change breaks module separation, it will be rejected in review.
