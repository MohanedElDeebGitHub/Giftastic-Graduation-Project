# Giftastic UI Entity Model Unification Plan

## Status and authority — audited 2026-06-20

This document is the immutable implementation specification. An implementation agent must not edit
its requirements, weaken its gates, or change its status. Evidence and progress belong in the
separate artifacts defined below.

The repository is **not considered complete** merely because entity folders exist, tests pass, or the
production build succeeds. The current implementation must be evaluated against the evidence gates in
this document. At the time of this revision, the repository contains substantial entity-model
foundations, but Phase 1 and Phase 2 have not been proven complete.

There are two distinct completion scopes:

- `FRONTEND_COMPLETE`: every frontend requirement and verification gate in this document is
  satisfied against the current, audited backend contract.
- `SYSTEM_CERTIFIED`: `FRONTEND_COMPLETE` is satisfied and every item in the Backend Contract
  Register that affects security or behavior has also been resolved and verified in the backend.

This assignment targets `FRONTEND_COMPLETE`. Backend changes are not authorized unless the user
explicitly expands the scope. An unresolved backend issue must be recorded and must not be hidden by
invented frontend behavior. It blocks `SYSTEM_CERTIFIED`, but it blocks `FRONTEND_COMPLETE` only when
no safe and truthful frontend behavior can be implemented against the current backend contract.

Frontend status may be reported as `FRONTEND_COMPLETE` only when:

- every mandatory row in the Entity Compliance Ledger has implementation evidence;
- every production representation in the Migration Inventory is marked migrated or approved as an
  explicit exception;
- all required automated and browser tests pass;
- every repository-wide cleanup query satisfies its zero-result or allowlist rule;
- a fresh adversarial audit finds no unresolved requirement, regression, privacy leak, or legacy
  representation;
- the person or agent performing the final audit did not rely only on the implementation's own
  completion claims.

Until all of those conditions are met, the only valid frontend status is `FRONTEND_INCOMPLETE`.

Unqualified statements such as “the system is complete” or “100% complete” are prohibited. Reports
must explicitly say either `FRONTEND_COMPLETE` or `SYSTEM_CERTIFIED`.

## Required evidence artifacts

The implementation must create and maintain these files without modifying this specification:

```text
docs/entity-ui-unification/
  BASELINE.md
  COMPLIANCE_LEDGER.md
  MIGRATION_INVENTORY.md
  EXCEPTIONS.md
  BACKEND_BLOCKERS.md
  VERIFICATION.md
  UI_QUALITY.md
  ADVERSARIAL_AUDIT.md
```

- `BASELINE.md`: branch, commit, worktree state, audit date, frontend routes, service methods, backend
  domain/DTO/controller sources, and the commands used to discover them.
- `COMPLIANCE_LEDGER.md`: the 23-entity evidence ledger.
- `MIGRATION_INVENTORY.md`: one concrete row for every production representation.
- `EXCEPTIONS.md`: reviewed exceptions only.
- `BACKEND_BLOCKERS.md`: backend observations; no backend edits without authorization.
- `VERIFICATION.md`: test, build, cleanup-query, and browser evidence.
- `UI_QUALITY.md`: per-view accessibility, responsive, state, design-system, and visual-regression
  evidence.
- `ADVERSARIAL_AUDIT.md`: findings from the final disproof-oriented audit and their resolution.

Missing, stale, or internally inconsistent evidence artifacts mean `FRONTEND_INCOMPLETE`.

Every evidence entry must include the tested commit, command or browser action, timestamp, exit
status/result, and relevant file or route. Summary statements such as “tests passed” without the
underlying commands and results are not evidence.

## Purpose

Giftastic’s backend domain classes are the source of truth for:

- what an entity is;
- which fields belong to it;
- which child records and relationships belong to it;
- which states it can enter;
- which operations it supports;
- which identifiers establish ownership.

The frontend currently represents those entities independently in pages, cards, tables, forms, dashboards, and modals. Those representations frequently use different fields, formatting, visibility rules, and actions.

This plan has two strict phases:

1. **Phase 1 — Create the canonical UI entity models.**
2. **Phase 2 — Replace and validate existing views using those models.**

No broad view migration should begin for an entity until its Phase 1 model is complete.

---

# Normative Execution Contract

Everything in this section is mandatory. Later examples may explain these rules, but they may not
weaken them.

## 1. Closed scope

The plan covers every persistent backend domain entity that can be represented in the frontend:

1. User, including Admin, Super Admin, Vendor, and review-restriction account facets
2. Vendor
3. Product
4. Order
5. Gift Flow
6. Cart
7. Review
8. Category
9. Vendor Application
10. Commission
11. Commission Payment Request
12. Commission Rule
13. Report
14. Admin Request
15. Order Assistance Request
16. Notification
17. Vendor Feedback
18. Delivery Zone
19. Vendor Delivery Pricing
20. Reminder
21. Vendor Activity
22. User Review Restriction
23. Favorite Product/Flow relationship

The following embedded backend value objects are mandatory parts of their parent model:

- Address → User
- Admin → User Admin facet
- Product Details and Product Image → Product
- Order Item and Guest Info → Order
- Cart Item → Cart
- Order Assistance Message → Order Assistance Request
- Vendor Delivery Pricing ID → Vendor Delivery Pricing composite identity

The following are not independent domain entities, but they are still centralized UI contracts:

- authentication/session viewer projection;
- unified search projections;
- product search projections;
- recommendations;
- platform analytics;
- vendor analytics;
- financial analytics;
- create/edit command drafts.

They must live in an explicit projection or command domain and may not redefine canonical entities.
Whenever a projection contains an entity reference, that reference must be adapted through the
referenced entity's adapter or normalized snapshot contract.

## 2. Definitions

### Canonical entity model

A canonical entity model is a runtime representation of one backend domain identity. It contains:

- the complete declared field vocabulary for that entity;
- normalized values for fields supplied by the source payload;
- typed embedded values and typed relations;
- source and adapter identity;
- loaded-field metadata;
- partial/completeness metadata;
- no viewer-specific permission decision;
- no modal, page, table, or visual-layout state.

The canonical model is not required to have every value loaded at runtime. It is required to know
every legal field and to distinguish whether each field was loaded.

### Entity reference

A relation to another entity is represented as:

```js
{
  entityType,
  id,
  snapshot,       // optional normalized partial model
  loaded          // whether the snapshot was supplied
}
```

Names, email addresses, or labels must never be used as identity or ownership keys.

### Projection

A projection is a backend response designed for search, analytics, recommendations, or another
specific workflow. A projection may add metrics, rankings, or labels, but it must not become an
alternative definition of an entity.

### Command draft

A command draft is mutable form state used to create or update an entity. It is not a persisted
entity and does not use entity field-state semantics. It must still import canonical field names,
enums, validation rules, and payload mapping from the entity or command domain.

### Presentation

A presentation is a semantic `Summary`, `Row`, `Card`, `Details`, `Editor`, or justified workflow
view. A modal, page, drawer, or popover is only a container around one or more presentations.

## 3. One shared runtime contract

All 23 top-level models must expose equivalent semantics, even when their internal implementation is
generated from schemas:

```js
{
  entityType,
  schemaVersion,
  identity,
  data,
  relations,
  meta: {
    source,
    adapter,
    loadedFields,
    isPartial,
    fetchedAt,
    issues
  }
}
```

Exact object layout may differ only when documented and covered by shared contract tests. The
following operations must exist consistently:

```js
createEntityModel()
adaptEntityFromNamedSource()
mergeEntityModels()
hasLoadedEntityField()
getEntityFieldState()
readEntityField()
buildEntityAccess()
buildEntityActions()
```

User, Vendor, and Product may have richer typed helpers, but they may not implement incompatible
field-state, merge, viewer, or permission semantics.

Mechanically identical behavior must use shared factories, helpers, or schema-driven generation.
Creating 23 copied versions of field-state, merge, relation, permission-set, action, or adapter
plumbing violates this plan even when the files have different entity names.

## 3.1 Scalar, parsing, and validation contract

Canonical models normalize backend scalar values consistently:

- UUIDs and other identifiers → strings without truncation;
- `LocalDate` → ISO date-only string (`YYYY-MM-DD`);
- `LocalDateTime` → ISO date-time string with its backend meaning preserved; formatting and timezone
  conversion happen only in selectors;
- `BigDecimal` currency/rates → normalize immediately to a canonical decimal string or decimal
  library and avoid subsequent binary floating-point arithmetic. When the JSON parser has already
  delivered a JavaScript number, do not claim precision that the transport did not preserve; record
  any financial-scale limitation as a backend contract issue;
- enums → exact centralized enum values;
- booleans → preserve explicit `false`;
- structured JSON strings → preserve the raw value and expose a safe parsed result.

Adapters validate source types and enum values. Malformed values do not become ordinary canonical
values. They produce a structured `meta.issues` entry containing the source, field path, reason, and
severity. A malformed field remains unavailable to normal selectors unless a documented safe
fallback exists.

Parsing rules:

- JSON parsing never throws during rendering;
- invalid metadata/configuration is distinguishable from unloaded and intentionally empty data;
- unknown enum values render through one centralized “unknown state” selector and are recorded as an
  issue;
- invalid dates and decimals are never displayed as `Invalid Date`, `NaN`, or fabricated zero;
- URLs used for images, links, or proof files pass centralized URL/scheme validation;
- entity presentations never use `dangerouslySetInnerHTML` for backend/user content.

## 4. Field-state contract

Every declared field must resolve to exactly one state:

| State | Meaning | Rendering rule |
| --- | --- | --- |
| `AVAILABLE` | Loaded and contains a meaningful value, including `false` or `0` | May render |
| `EMPTY` | Loaded and intentionally `null`, blank, or empty | May render an intentional empty state |
| `UNLOADED` | Source payload did not contain the field | Do not imply emptiness; hydrate only if justified |
| `INVALID` | Source supplied the field, but its type/value could not be safely normalized | Do not render as ordinary data; expose a controlled error/fallback |
| `FORBIDDEN` | Access policy denies the field in this context | Do not render, log, serialize, or hydrate for display |

State precedence is:

```text
FORBIDDEN → UNLOADED → INVALID → EMPTY → AVAILABLE
```

Malformed source values are `INVALID`, are recorded in `meta.issues`, and do not masquerade as
`EMPTY`, `UNLOADED`, or `AVAILABLE`.

Mandatory behavior:

- `false`, `0`, and an empty array supplied by the backend are loaded values, not missing values.
- Adapters may not add fake defaults to make a view convenient.
- Selectors may provide display fallbacks only after checking field state.
- Access calculation does not delete data from the model; the reader combines model state and access.
- A view may not directly read a protected field from the raw model.
- Detailed hydration may request only fields/endpoints the viewer is authorized to retrieve.

## 5. Named adapters only

Every response shape consumed by the frontend must have a named adapter. Examples:

```text
adaptUserDomain
adaptUserPublicProfile
adaptUserAuthSession
adaptOrderDomain
adaptOrderCustomerListRecord
adaptProductSearchResult
adaptProductAnalyticsReference
adaptVendorUnifiedSearchResult
```

Requirements:

- each service method or endpoint is mapped to one named adapter;
- aliases such as `categoryName`, `name`, `productName`, `customerName`, `flowId`, and `vendorId`
  are resolved only in adapters;
- views must not use `a || b` or `a ?? b` to interpret multiple DTO aliases;
- adapters must be idempotent when passed an already-canonical model;
- adapters must declare whether the source is complete or partial;
- adapter tests must enumerate the exact loaded fields for partial sources;
- unknown fields are not silently promoted into the canonical schema;
- backend-enriched labels remain enrichment fields or relation snapshots, not intrinsic identity.

## 6. Viewer and account contract

There is exactly one viewer factory and one application-level viewer source.

```js
{
  userId,
  supplierId,
  roles,
  permissions,
  isAuthenticated,
  isVendor,
  isAdmin,
  isSuperAdmin,
  facets: {
    vendor,
    admin
  }
}
```

Rules:

- Guest is the absence of an authenticated User.
- Every authenticated account is a User.
- Vendor, Admin, community helper, and Super Admin are additive User facets.
- A User may have all facets simultaneously.
- `SUPER_ADMIN` implies every permission but does not remove User or Vendor ownership.
- `roles` express broad account membership; `permissions` express fine-grained Admin authority.
- Authentication authorities that contain permission strings must be normalized into `permissions`.
- No page, component, modal, hook, or route may reconstruct its own viewer object.
- No screen may infer capabilities from a single primary `role`.
- The canonical viewer is created at the session boundary and consumed everywhere else.

## 7. Access and context contract

Every entity owns one access builder:

```js
buildEntityAccess({ entity, viewer, context, relationship })
```

The result must include:

- `canRead`;
- ownership and participation facts;
- section visibility;
- field visibility;
- action prerequisites;
- the normalized permission set used for the decision.

Contexts are selected from a centralized vocabulary:

```text
PUBLIC
SEARCH
SUMMARY
SELF
OWNER_MANAGEMENT
ORDER_CUSTOMER
ORDER_VENDOR
CHECKOUT
ADMIN_READ
ADMIN_MODERATION
ADMIN_FINANCIAL
SYSTEM
EDIT
```

Entity-specific contexts may extend this list but must be declared and tested.

Context can only reduce information. Context cannot grant permission.

Frontend access is a presentation boundary, not a security boundary. Sensitive data must still be
authorized and minimized by the backend. The UI access layer prevents accidental display and keeps
representations consistent; it must never be described as sufficient protection for data already
delivered to an unauthorized client.

Every field and section is classified in its entity schema:

```text
PUBLIC
AUTHENTICATED
SELF
OWNER
PARTICIPANT
ADMIN_PERMISSION
FINANCIAL
MODERATION
SYSTEM
```

Unclassified fields default to forbidden. Public visibility must always be explicit.

At minimum, the following are never public by default:

- User email, phone, birthday, addresses, ban state, permissions, and restrictions;
- Product inventory, Vendor notes/SKU, moderation state, and system timestamps/IDs where not needed;
- Order customer/guest, shipping, payment, delivery breakdown, Vendor participation, and financial data;
- Commission, payment proof, Commission Rules, and financial analytics;
- Vendor Application review data and applicant identity;
- Report administration and Vendor Feedback;
- review moderation fields and anonymous-author identity;
- notification ownership/metadata, reminders, and favorites;
- reviewer/resolver/Admin identifiers and system IDs.

All ownership comes from backend identifiers:

- User self → viewer User ID equals User/customer ID;
- Vendor owner → viewer supplier ID equals Vendor/supplier ID;
- Product owner → viewer supplier ID equals Product supplier ID;
- Order customer → viewer User ID equals Order customer ID;
- Order Vendor → viewer supplier ID appears on at least one Order Item;
- application/request/report/reminder/favorite/notification owner → exact backend owner ID;
- Admin and Super Admin authority → exact permission, never an unrelated permission.

## 8. Action contract

Actions are data derived by the entity domain. Views do not reconstruct legal transitions.

An action is emitted only when:

1. the viewer satisfies the exact backend-authorized capability set—ownership and/or one of the
   explicitly supported permissions—without an unrelated permission implying access;
2. the entity's loaded state permits the backend operation;
3. the relevant state fields are loaded;
4. the consuming controller supplied a handler.

Every action definition includes:

```js
{
  key,
  label,
  intent,
  confirmation,
  disabledReason,
  onSelect
}
```

If backend permission names, endpoint checks, or state transitions disagree, the action remains
blocked and the mismatch is recorded in the Backend Contract Register. The frontend must not guess.

Controllers may invoke canonical `buildEntityAccess` and `buildEntityActions` functions. What is
forbidden is reproducing, extending, or overriding their decision logic in a page, modal, section,
route, or event handler.

## 9. Hydration and caching contract

Semantic sections and presentation components never fetch.

Hydration belongs in:

- page/controller hooks;
- route loaders;
- entity repositories/query hooks;
- a shared cached hydration service.

Requirements:

- a presentation declares required field paths;
- the controller compares required paths with loaded fields;
- hydration is authorized before the request;
- results merge through canonical model merge logic;
- related entities are hydrated through their own repositories;
- repeated references use caching or batching;
- list rendering must not trigger one request per row;
- a failed hydration preserves the partial model and reports a non-destructive error;
- modal open/close behavior must not own domain fetching logic.

## 10. Presentation contract

Every entity must have the semantic views actually required by the Migration Inventory. At minimum:

- a `Summary` or decoration view when the entity appears in lists;
- `Details` when a full representation exists;
- `Row` or `Card` when its behavior differs materially from `Summary`;
- `Editor` only when the entity has an existing edit workflow;
- specialized workflow presentations only when justified.

Presentations:

- accept canonical models, access results, and handlers;
- use selectors and field readers;
- do not adapt raw payloads;
- do not fetch;
- do not inspect raw roles or permission strings;
- do not recreate state transitions;
- do not render arbitrary objects with `JSON.stringify`;
- do not contain modal overlay behavior;
- compose related entities through their canonical summaries.

Modals, pages, drawers, and popovers may control layout, navigation, focus, loading, and closing only.

## 10.1 Professional UI quality contract

Architectural unification is not sufficient if the resulting production UI is inconsistent,
inaccessible, or visibly degraded. Every migrated semantic view must comply with
`frontend/giftastic_design_system/DESIGN.md` and the existing Tailwind design tokens.

This is a professionalization pass, not authorization for an unrelated product redesign. Existing
workflows, information hierarchy, navigation, and user capabilities are preserved unless the
Migration Inventory explicitly records a justified correction.

### Design-system consistency

- use the shared Giftastic color, typography, spacing, radius, and elevation tokens;
- do not introduce one-off hex colors, arbitrary typography, duplicated shadows, or unrelated visual
  systems inside entity views;
- equivalent entity states and actions use the same visual language everywhere;
- destructive, primary, secondary, disabled, warning, verified, pending, rejected, and success
  treatments are centralized;
- compact and detailed variants remain recognizably the same entity representation;
- responsive layouts preserve content priority rather than merely shrinking desktop markup.

### Required view states

Every asynchronous or partial entity presentation must deliberately support the states that can
occur:

```text
loading
partial/loading-more
ready
empty
invalid-data
forbidden/omitted
recoverable-error
action-pending
action-success
action-error
```

Views must not flash forbidden information, display fake placeholders as real data, collapse
unloaded into empty, or leave controls active during duplicate submissions.

### Accessibility

Migrated production views target WCAG 2.2 AA for applicable behavior:

- complete keyboard operation and logical tab order;
- visible focus indicators;
- semantic headings, lists, tables, buttons, links, labels, and form errors;
- accessible names for icon-only controls;
- modal focus entry, containment, Escape handling, close control, and focus restoration;
- status and asynchronous updates announced when necessary;
- color is not the sole carrier of status or meaning;
- sufficient text/control contrast;
- meaningful image alternatives and decorative images hidden from assistive technology;
- touch targets and controls remain usable on mobile.

### Responsive and visual-regression requirements

At minimum, representative views are verified at:

- narrow mobile: approximately 320–375 CSS pixels;
- tablet: approximately 768 CSS pixels;
- desktop: approximately 1280 CSS pixels.

Verification must check overflow, clipping, wrapping, table/card adaptation, modal height and scroll,
sticky controls, long names/IDs, empty values, large item counts, and long translated/user-entered
content.

Before replacing a production view, capture or document its functional baseline. After migration,
record comparable evidence in `UI_QUALITY.md`. A deliberate visual improvement is allowed; loss of
information, actions, usability, responsive behavior, or accessibility is a regression.

The `Semantic views` ledger cell is incomplete until both architectural evidence and this UI quality
evidence exist for the entity's required variants.

## 11. Centralization and no-duplication rule

A change to an entity's field name, display formatting, badge, visibility, ownership, or action rule
must be implemented once in that entity domain and reflected in every consuming view.

The following are forbidden outside the owning entity/projection/command domain:

- DTO alias normalization;
- status-to-label/style maps;
- entity-specific date, money, rating, and identifier formatting;
- permission-string interpretation;
- ownership calculations;
- legal transition calculations;
- defaulting unloaded fields;
- duplicate entity cards/details markup.

Shared cross-entity primitives may live under `ui/entities/shared`, but shared primitives must not
replace semantic entity presentations with generic JSON-like output.

---

# Core Rules

## 1. Backend domain classes are authoritative

Each frontend entity model starts from its corresponding backend domain class.

Backend DTOs and API responses are delivery shapes. They may provide:

- the complete entity;
- a public projection;
- a protected projection;
- an analytics projection;
- a partial search/list record;
- an entity enriched with related labels.

DTOs do not redefine the entity. They are adapted into the canonical frontend entity model.

The frontend model must not invent fields, states, relationships, or actions unsupported by the backend domain.

## 2. UI model and visual view are different things

The canonical UI entity model contains:

- normalized entity data;
- loaded-field information;
- optional-field handling;
- derived values;
- viewer relationship and ownership;
- RBAC-derived visibility;
- legal actions for the current viewer and entity state.

It is not a modal, card, table row, or page.

Views are built from the model later:

```text
Backend domain
      ↓
API response adapter
      ↓
Canonical UI entity model
      ↓
Viewer + context access calculation
      ↓
Card / row / page / form / modal / drawer
```

## 3. User is the account archetype

All accounts are Users.

- A vendor is a User with a Vendor facet.
- An admin is a User with an Admin facet and permissions.
- A super admin is a User/Admin whose permissions contain `SUPER_ADMIN`.
- A single User may simultaneously be a customer, vendor, admin, community helper, or super admin.

These facets are additive, not mutually exclusive.

The frontend must not use a single exclusive `role` value to decide what an account is allowed to see or do.

The canonical account model is:

```text
User UI model
├── core User data
├── optional Vendor facet
├── optional Admin facet
│   └── permissions[]
├── optional review-restriction facet
└── derived capabilities
```

Admin and Super Admin may have specialized sections and views, but they do not replace User identity.

## 4. Super admin and admin behavior

- `SUPER_ADMIN` grants every permission.
- A normal admin receives only explicitly granted permissions.
- Permissions are additive.
- Having one admin permission must not imply unrelated read or write access.
- Vendor ownership, User self-access, and admin permissions may all apply to the same viewer simultaneously.

## 5. Viewer and viewing context are separate inputs

Information visibility depends on:

1. **Who is viewing**
   - guest;
   - authenticated User;
   - entity owner;
   - participating Vendor;
   - Admin with specific permissions;
   - Super Admin.

2. **Where and why the entity is being viewed**
   - public browse;
   - search result;
   - self profile;
   - vendor-owned management;
   - order fulfilment;
   - admin moderation;
   - financial administration;
   - system/debug details.

Context may reduce the fields shown, but it must never grant access.

Example:

```text
Permission allows contact data
        +
Current context requests contact data
        +
Contact fields were loaded
        =
Contact section may render
```

## 6. Field states must remain distinct

For every field, the UI model must distinguish:

- **available:** loaded and contains a value;
- **empty:** loaded but intentionally empty/null;
- **unloaded:** not included in the current API response;
- **invalid:** included but not safely normalizable;
- **forbidden:** viewer is not allowed to read it.

These states cannot be collapsed into `undefined`, an empty string, or a fake default.

Examples:

- An unloaded commission amount is not zero.
- A forbidden phone number is not “Not provided.”
- An anonymous review author must remain hidden even if a User ID is present.

## 7. Partial data is valid

Search results, table rows, analytics records, order item snapshots, and embedded references may contain only part of an entity.

The canonical model must:

- normalize the fields that are present;
- record which fields were loaded;
- remain usable for compact views;
- never pretend to be complete;
- support controlled hydration when a detailed view requires more data.

Hydration is handled by the consuming controller or data layer. Individual entity sections must not independently fetch data.

## 8. Access is calculated centrally

Each entity receives one access builder:

```js
buildEntityAccess({
  entity,
  viewer,
  context
})
```

It returns capabilities such as:

```js
{
  canViewIdentity: true,
  canViewContact: false,
  canViewFinancials: true,
  canEdit: false,
  canApprove: true,
  canViewSystemIds: false
}
```

Views consume these capabilities. They do not repeatedly interpret raw permission strings.

## 9. Actions require three conditions

An action is available only when:

1. the viewer has permission or ownership;
2. the backend entity’s current state allows the operation;
3. the consuming view supplied an action handler.

The frontend may mirror backend state transitions for presentation, but the backend remains authoritative.

---

# Shared Phase 1 Contracts

Before individual entity models are created, establish these shared contracts.

## Viewer model

```js
{
  userId,
  supplierId,
  roles,
  permissions,
  isAuthenticated,
  isVendor,
  isAdmin,
  isSuperAdmin
}
```

Rules:

- `isSuperAdmin` comes from `SUPER_ADMIN`.
- `isAdmin` comes from the Admin facet/permissions.
- `isVendor` comes from the Vendor facet or `supplierId`.
- no screen constructs its own interpretation of the viewer.

## Viewing contexts

Use a controlled set of semantic contexts, for example:

```text
PUBLIC
SEARCH
SUMMARY
SELF
OWNER_MANAGEMENT
ORDER_CUSTOMER
ORDER_VENDOR
ADMIN_READ
ADMIN_MODERATION
ADMIN_FINANCIAL
SYSTEM
EDIT
```

Entity domains may define narrower sub-contexts when required.

Contexts describe presentation intent. They do not replace permissions.

## Permission constants

The frontend permission list must be centralized and mirror backend `AdminPermission`:

### Users

- `VIEW_USERS`
- `MANAGE_USERS`
- `DELETE_USERS`
- `BAN_USERS`
- `UNBAN_USERS`
- `REVIEW_ADMIN_REQUESTS`

### Admins

- `MAKE_ADMINS`
- `DEMOTE_ADMINS`
- `MANAGE_ADMIN_PERMISSIONS`

### Vendors

- `MAKE_VENDORS`
- `ACTIVATE_VENDORS`
- `DEACTIVATE_VENDORS`

### Products

- `ACTIVATE_PRODUCTS`
- `REJECT_PRODUCTS`
- `DEACTIVATE_PRODUCTS`
- `DELETE_PRODUCTS`

### Categories

- `MANAGE_CATEGORIES`

### Orders

- `VIEW_ORDERS`
- `MANAGE_ORDERS`
- `MANAGE_ORDER_STATUS`
- `REVIEW_ORDER_ASSISTANCE`

### Financial

- `VIEW_FINANCIAL_DATA`
- `MANAGE_COMMISSIONS`
- `REVIEW_COMMISSION_PAYMENTS`
- `URGE_COMMISSION_PAYMENT`
- `VIEW_FINANCIAL_ANALYTICS`

### Gift flows

- `MANAGE_GIFT_FLOWS`

### Platform and moderation

- `SEND_NOTIFICATIONS`
- `MANAGE_REPORTS`
- `VIEW_REVIEWS`
- `MODERATE_REVIEWS`
- `VIEW_VENDOR_FEEDBACK`
- `MUTE_USERS`

### Global

- `SUPER_ADMIN`

## Ownership rules

The shared access layer must support:

- User self: `viewer.userId === entity.userId/id`
- Vendor owner: `viewer.supplierId === entity.supplierId/vendorId`
- Product owner: Product supplier matches viewer supplier
- Application owner: application User matches viewer User
- Order customer: order customer matches viewer User
- Order Vendor: viewer supplies one or more order items
- Notification owner
- Reminder owner
- Favorite owner

Ownership must be determined from backend identifiers, never names or email addresses.

## Required files for each UI entity model

Each top-level entity model must have:

```text
entities/<entity>/
  <entity>Schema.js
  <entity>Model.js
  <entity>Adapters.js
  <entity>Access.js
  <entity>Selectors.js
  <entity>Actions.js
  views/
    <Entity>Summary.jsx       # when required by the migration inventory
    <Entity>Row.jsx           # when required
    <Entity>Card.jsx          # when required
    <Entity>Details.jsx       # when required
    <Entity>Editor.jsx        # when an edit workflow exists
    sections/
  __tests__/
    <entity>Adapters.test.js
    <entity>Access.test.js
    <entity>Actions.test.js
    <entity>Views.test.jsx
  index.js
```

Responsibilities:

- `Schema`: exhaustive field registry, types, enums, sensitivity, embedded values, relations, and
  source-of-truth backend references. Each field declares its provenance as `INTRINSIC`,
  `EMBEDDED`, `RELATION`, `ENRICHMENT`, `PROJECTION`, or `DERIVED`.
- `Model`: canonical field shape and loaded-field metadata.
- `Adapters`: one named adapter for every service/endpoint/projection shape.
- `Access`: viewer/context/RBAC and ownership decisions.
- `Selectors`: derived values and all entity-specific display formatting.
- `Actions`: legal action definitions based on capability and entity state.
- `views`: reusable semantic presentations independent from modal/page containers.
- `tests`: source-shape, field-state, viewer-matrix, transition, and rendering tests.

A `DERIVED` field is available only when every required source field is loaded, valid, and readable.
It becomes `UNLOADED` when its inputs are unloaded, and `FORBIDDEN` when deriving it would reveal a
forbidden input. Derived fields are never added to `loadedFields` as though they came from the
backend.

An entity may omit a visual variant only when the Migration Inventory proves that variant is not
used. It may not omit `Schema`, `Model`, `Adapters`, `Access`, `Selectors`, `Actions`, or tests.

Embedded value objects may live below their parent entity, but must have their own schema, adapter,
selector, and tests when they contain multiple fields or parsing logic.

Cross-entity analytics and search projections must use the parallel structure:

```text
ui/projections/<projection>/
  <projection>Schema.js
  <projection>Adapters.js
  <projection>Selectors.js
  views/
  __tests__/
```

Command forms must use:

```text
ui/commands/<command>/
  <command>Schema.js
  <command>Draft.js
  <command>Validation.js
  <command>Payload.js
  __tests__/
```

---

# PHASE 1 — Create Every Canonical UI Entity Model

Phase 1 creates the entity model layer only.

It does **not** broadly replace pages, cards, tables, or modals.

Small temporary harnesses, stories, or tests may render the model for verification, but production view migration belongs to Phase 2.

## Phase 1 implementation order

Priority is based on entity size, usage, dependencies, privacy risk, and business importance.

| Priority | UI entity model | Reason |
| ---: | --- | --- |
| 1 | User with account facets | Root identity used by almost every protected domain |
| 2 | Vendor | User facet referenced by products, flows, orders, commissions, delivery, and reviews |
| 3 | Product | Most widely represented commercial entity |
| 4 | Order | Most complex transactional and permission-dependent entity |
| 5 | Gift Flow | Structured entity composed from Products and Vendor ownership |
| 6 | Cart | Critical Product-to-checkout aggregate |
| 7 | Review | Public, self, anonymous, and moderator representations |
| 8 | Category | Small but foundational Product taxonomy |
| 9 | Vendor Application | Sensitive User-to-Vendor workflow |
| 10 | Commission | Financial Vendor/Order record |
| 11 | Commission Payment Request | Financial proof and approval workflow |
| 12 | Commission Rule | Protected financial configuration |
| 13 | Report | Cross-entity moderation record |
| 14 | Admin Request | User-to-Admin workflow |
| 15 | Order Assistance Request | Vendor/Admin threaded Order workflow |
| 16 | Notification | User-owned message with polymorphic metadata |
| 17 | Vendor Feedback | Private moderation record |
| 18 | Delivery Zone | Checkout and delivery reference entity |
| 19 | Vendor Delivery Pricing | Vendor-owned relation between Vendor and Delivery Zone |
| 20 | Reminder | User-owned scheduled entity |
| 21 | Vendor Activity | Vendor-owned event entity |
| 22 | User Review Restriction | Protected User moderation facet |
| 23 | Favorite Product/Flow | User-owned relationship model |

Embedded backend value objects are created with their parent entity:

- Address with User;
- Product Details and Product Image with Product;
- Order Item and Guest Info with Order;
- Cart Item with Cart;
- Order Assistance Message with Order Assistance Request.

---

## 1. User UI model

### Backend sources

- `User`
- `Address`
- linked `Admin`
- linked `Vendor`
- linked `UserReviewRestriction`
- `PublicUserProfileResponse`
- authentication and admin User projections

### Canonical data

#### Core User

- `id`
- `email`
- `fullName`
- `phoneNumber`
- `birthday`
- `addresses`
- `isBanned`
- `requestedAdmin`
- public membership date when supplied

`passwordHash` is never part of the frontend model.

#### Vendor facet

- `isVendor`
- `supplierId`
- optional linked Vendor reference

#### Admin facet

- `isAdmin`
- `permissions`
- `isSuperAdmin`
- optional community-helper derivation

#### Review-restriction facet

- `canComment`
- `canReview`
- `restrictedAt`
- `restrictedBy`
- `reason`
- `expiresAt`
- `isActive`

### Access rules

- Guest/public context: public identity only.
- Self: own profile, contact, addresses, and applicable account history.
- `VIEW_USERS`: protected User details.
- `MANAGE_USERS`: administrative profile changes.
- `BAN_USERS`, `UNBAN_USERS`, `DELETE_USERS`: exact actions.
- `MAKE_ADMINS`, `DEMOTE_ADMINS`: Admin facet actions.
- `MANAGE_ADMIN_PERMISSIONS`: permission visibility and mutation.
- `MUTE_USERS`: review restriction management.
- Super Admin: complete readable model and all legal actions.

### Required semantic sections

- identity;
- public account badges;
- contact;
- addresses;
- account status;
- Vendor facet;
- Admin/permission facet;
- review restriction;
- system identifiers;
- actions.

---

## 2. Vendor UI model

### Backend source

`Vendor`

### Canonical data

- `userId`
- `supplierId`
- `storeName`
- `description`
- `logoUrl`
- `bannerUrl`
- `contactEmail`
- `contactPhone`
- `address`
- `websiteUrl`
- `instagramUrl`
- `facebookUrl`
- `workingHours`
- `isVerified`

Related User, Products, Gift Flows, Reviews, analytics, and delivery pricing are optional loaded relations, not intrinsic Vendor fields.

### Access rules

- Public: verified public storefront data.
- Vendor owner: own management fields and owner actions.
- `ACTIVATE_VENDORS`: activation.
- `DEACTIVATE_VENDORS`: deactivation.
- financial permissions: only explicitly loaded financial projections.
- Super Admin: all loaded fields and system identifiers.

### Required sections

- storefront identity;
- description;
- contact and location;
- social links;
- status;
- owner reference;
- loaded related-entity summaries;
- system identifiers;
- actions.

---

## 3. Product UI model

### Backend sources

- `Product`
- `ProductDetails`
- `ProductImage`
- Category relationship
- Product search and analytics projections

### Canonical data

- identity and Vendor ownership;
- name and description;
- original/current price;
- status and lifecycle timestamps;
- rating and review count;
- stock;
- discount fields;
- Categories;
- images and primary image;
- all Product Details fields:
  - add-on pricing;
  - video;
  - personalization;
  - colors and sizes;
  - gift presentation;
  - delivery and perishability;
  - recipient requirements;
  - composition;
  - marketing;
  - SEO;
  - Vendor SKU, notes, and fulfilment.

### Access rules

- Public: approved commerce and public descriptive fields.
- Vendor owner: draft/internal fields, stock, SKU, Vendor notes, and owner operations.
- product moderation permissions: exact approve, reject, deactivate, and delete operations.
- Super Admin: all loaded fields.

### Required sections

- identity/media;
- pricing;
- rating;
- inventory;
- taxonomy;
- personalization;
- gift presentation;
- delivery;
- recipient requirements;
- composition;
- marketing;
- Vendor-only data;
- SEO/system data;
- actions.

---

## 4. Order UI model

### Backend sources

- `Order`
- `OrderItem`
- `GuestInfo`
- order security projection

### Canonical data

- identity, status, and placement time;
- registered customer reference or Guest Info;
- items;
- captured item prices;
- Vendor participation per item;
- shipping and delivery;
- payment;
- total and delivery cost;
- delivery estimate/actual dates and notes;
- commission-paid state;
- optional loaded commissions and assistance requests.

### Access rules

- customer owner;
- participating Vendor;
- `VIEW_ORDERS`;
- `MANAGE_ORDERS`;
- `MANAGE_ORDER_STATUS`;
- `REVIEW_ORDER_ASSISTANCE`;
- `VIEW_FINANCIAL_DATA`;
- Super Admin.

Customer and Vendor access must reveal only the fields required for their relationship to the Order.

### Required sections

- header/status;
- customer or guest;
- items;
- personalization metadata;
- totals;
- shipping;
- delivery;
- payment;
- Vendor fulfilment;
- commission state;
- loaded assistance references;
- system identifiers;
- actions.

---

## 5. Gift Flow UI model

### Backend source

`GiftFlow`

### Canonical data

- `id`
- `supplierId`
- `name`
- `description`
- `imageUrl`
- `createdAt`
- `updatedAt`
- parsed configuration;
- normalized steps;
- selection rules;
- Product references and min/max constraints.

### Access rules

- Public: executable customer flow data.
- Vendor owner: edit/delete and internal configuration.
- `MANAGE_GIFT_FLOWS`: administrative management.
- Super Admin: raw configuration and system data.

### Required sections

- identity;
- journey/steps;
- Product choices;
- selection constraints;
- Vendor reference;
- raw configuration when authorized;
- system data;
- actions.

---

## 6. Cart UI model

### Backend sources

- `Cart`
- `CartItem`
- `CartResponse`

### Canonical data

- `id`
- `customerId`
- `items`
- total;
- update time when supplied.

Cart Item contains:

- Product reference and loaded Product summary;
- quantity;
- captured/displayed price;
- group ID;
- parsed personalization metadata;
- Vendor reference.

### Access rules

- User owner.
- Super Admin only if an explicit backend support endpoint allows it.

### Required sections

- item groups;
- Product summaries;
- personalization;
- Vendor grouping;
- quantities;
- totals;
- actions.

---

## 7. Review UI model

### Backend source

`Review`

### Canonical data

- identity;
- User reference;
- review type and target entity;
- rating;
- comment;
- moderation status;
- timestamps;
- reviewer and notes;
- anonymity;
- content score;
- Order reference.

### Access rules

- Public: approved content; author only when not anonymous.
- Self: own review and moderation status.
- `VIEW_REVIEWS`: protected moderation data.
- `MODERATE_REVIEWS`: approve/reject.
- Super Admin: all loaded data.

### Required sections

- rating/content;
- author;
- target;
- status;
- moderation;
- system data;
- actions.

---

## 8. Category UI model

### Backend source

`Category`

### Canonical data

- one normalized `id`;
- one normalized `name`;
- optional loaded Product relations or count.

Backend aliases such as `categoryId`, `categoryName`, and `name` are resolved in adapters.

### Access rules

- Public: identity and Product relationship.
- `MANAGE_CATEGORIES`: supported mutations.
- Super Admin: system identifiers.

### Required sections

- identity;
- Product relation summary;
- system data;
- actions.

---

## 9. Vendor Application UI model

### Backend source

`VendorApplication`

### Canonical data

- identity and applicant User;
- proposed Vendor profile fields;
- status;
- submission and review timestamps;
- reviewer;
- rejection reason.

### Access rules

- application owner;
- `MAKE_VENDORS`;
- `ACTIVATE_VENDORS`;
- Super Admin.

### Required sections

- applicant;
- proposed storefront;
- contact/location/social information;
- status/timeline;
- review outcome;
- reviewer/system data;
- actions.

---

## 10. Commission UI model

### Backend source

`Commission`

### Canonical data

- `id`
- `orderId`
- `supplierId`
- optional supplier name enrichment;
- order subtotal;
- rate;
- amount;
- status;
- due, paid, and created times;
- overdue derivation.

### Access rules

- Vendor owner;
- `VIEW_FINANCIAL_DATA`;
- `URGE_COMMISSION_PAYMENT`;
- Super Admin.

### Required sections

- amount/rate;
- Vendor and Order references;
- status;
- due/payment timeline;
- system data;
- actions.

---

## 11. Commission Payment Request UI model

### Backend source

`CommissionPaymentRequest`

### Canonical data

- request, Commission, and Vendor identifiers;
- Vendor enrichment;
- message;
- proof image URL;
- status;
- submission/review timeline;
- reviewer;
- rejection reason.

### Access rules

- submitting Vendor owner;
- `REVIEW_COMMISSION_PAYMENTS`;
- Super Admin.

Payment proof is protected financial evidence.

### Required sections

- request summary;
- proof;
- status/timeline;
- review information;
- system data;
- actions.

---

## 12. Commission Rule UI model

### Backend source

`CommissionRule`

### Canonical data

- identity;
- rule type;
- optional Vendor target;
- rate;
- start/end dates;
- active state;
- creator and creation date.

### Access rules

- `MANAGE_COMMISSIONS`;
- Super Admin.

### Required sections

- scope;
- rate;
- effective dates;
- active state;
- creator/system data;
- actions.

---

## 13. Report UI model

### Backend source

`Report`

### Canonical data

- reporter;
- report type;
- target entity reference;
- reason and description;
- status;
- creation/review timeline;
- reviewer;
- admin notes.

### Access rules

- reporter access where supported by backend endpoints;
- `MANAGE_REPORTS`;
- Super Admin.

### Required sections

- report content;
- reporter;
- typed target entity reference;
- status/timeline;
- administrative review;
- system data;
- actions.

---

## 14. Admin Request UI model

### Backend source

`AdminRequest`

### Canonical data

- request and User identity;
- message;
- status;
- request/review timeline;
- reviewer;
- review notes;
- reapplication cooldown.

### Access rules

- applicant owner;
- `REVIEW_ADMIN_REQUESTS`;
- `MAKE_ADMINS` for applicable approval/cooldown operations;
- Super Admin.

### Required sections

- applicant;
- request;
- status/timeline;
- outcome;
- cooldown;
- reviewer/system data;
- actions.

---

## 15. Order Assistance Request UI model

### Backend sources

- `OrderAssistanceRequest`
- `OrderAssistanceMessage`

### Canonical data

- request, Order, and Vendor identifiers;
- request message;
- status;
- request/resolution timeline;
- resolver and resolution;
- ordered message thread.

### Access rules

- participating Vendor;
- `REVIEW_ORDER_ASSISTANCE`;
- Super Admin.

### Required sections

- request summary;
- Order and Vendor references;
- thread;
- status/timeline;
- resolution;
- system data;
- actions.

---

## 16. Notification UI model

### Backend source

`Notification`

### Canonical data

- identity and owner User;
- title;
- message;
- type;
- read state;
- creation time;
- safely parsed metadata;
- typed related-entity reference when metadata permits.

### Access rules

- notification owner;
- Super Admin only where backend access explicitly supports it.

`SEND_NOTIFICATIONS` controls notification composition, not arbitrary reading of another User’s notifications.

### Required sections

- content;
- type/read state;
- related entity;
- metadata;
- system data;
- actions.

---

## 17. Vendor Feedback UI model

### Backend source

`VendorFeedback`

### Canonical data

- User, Vendor, and Order references;
- feedback;
- moderation status;
- timestamps;
- reviewer and notes;
- content score.

### Access rules

- submission workflow as explicitly allowed;
- `VIEW_VENDOR_FEEDBACK`;
- moderation endpoints/permissions;
- Super Admin.

Vendor Feedback is private moderation data and is not represented as a public Review.

### Required sections

- feedback;
- Vendor/Order references;
- moderation;
- system data;
- actions.

---

## 18. Delivery Zone UI model

### Backend source

`DeliveryZone`

### Canonical data

- `id`
- `zoneName`
- `description`
- `isActive`

### Access rules

- public/checkout: active zones;
- administrative/system contexts only where backend operations support them.

### Required sections

- identity;
- description;
- status;
- system data;
- actions.

---

## 19. Vendor Delivery Pricing UI model

### Backend source

`VendorDeliveryPricing`

### Canonical data

- Vendor ID;
- Delivery Zone ID;
- loaded zone name;
- delivery cost;
- update time.

### Access rules

- Vendor owner;
- Super Admin where supported.

### Required sections

- Vendor;
- zone;
- price;
- update time;
- actions.

---

## 20. Reminder UI model

### Backend source

`Reminder`

### Canonical data

- identity;
- owner User;
- description;
- scheduled time;
- processed state.

### Access rules

- User owner;
- Super Admin only where explicitly supported.

### Required sections

- description;
- schedule;
- state;
- actions.

---

## 21. Vendor Activity UI model

### Backend source

`VendorActivity`

### Canonical data

- identity;
- Vendor;
- activity type;
- description;
- related entity ID;
- safely parsed metadata;
- occurrence time.

### Access rules

- Vendor owner.

### Required sections

- activity identity;
- description;
- related entity;
- metadata;
- timeline/system data.

---

## 22. User Review Restriction UI model

### Backend source

`UserReviewRestriction`

### Canonical data

- User;
- comment/review capabilities;
- restriction time;
- restricting Admin;
- reason;
- expiry;
- active state.

### Access rules

- protected User moderation context;
- `MUTE_USERS` for mutation;
- Super Admin.

This model is normally composed as a protected User facet.

### Required sections

- current capabilities;
- restriction reason;
- timeline;
- administrator/system data;
- actions.

---

## 23. Favorite relationship UI model

### Backend source

`FavoriteProduct`

### Canonical data

- identity;
- owner User;
- optional Product ID;
- optional Gift Flow ID;
- added time.

Exactly one target type should be populated.

### Access rules

- User owner.

### Representation rule

The relationship decorates canonical Product or Gift Flow representations. It does not need an independent details view or modal.

---

## Phase 1 completion criteria

Phase 1 is complete only when every listed entity has objective evidence for every row below:

| Gate | Required evidence |
| --- | --- |
| Backend field parity | Schema test compares the declared field catalog with the audited domain/embedded fields |
| Enum parity | Every backend state/type enum is represented centrally and tested |
| Named source adapters | Adapter registry names every consumed endpoint/DTO/projection |
| Partial fidelity | Each partial adapter test asserts the exact loaded-field set |
| Field states | Available, empty, unloaded, invalid, and forbidden tests include `false`, `0`, empty lists, null, and malformed values |
| Merge behavior | Partial and hydrated models merge without erasing loaded values or inventing completeness |
| Viewer contract | Access uses the shared viewer and does not reinterpret roles locally |
| Ownership | Exact owner and unrelated-viewer tests use backend identifiers |
| Least privilege | Every relevant Admin permission is tested independently from unrelated permissions |
| Additive facets | User+Vendor+Admin and Super Admin combinations are tested |
| Context reduction | Public/search/summary contexts cannot expose protected fields even to privileged viewers |
| Actions | Permission, ownership, loaded state, backend state, and handler requirements are all tested |
| Selectors | Entity-specific formatting and derived values are centralized and tested |
| Semantic views | Every view variant required by the Migration Inventory exists outside modal/page folders |
| Professional UI | Required variants satisfy the design-system, state, accessibility, responsive, and visual-regression contract |
| No fetching | Entity models and semantic presentations contain no service/API calls |
| Public API | `index.js` exports only the supported entity contract |
| Contract tests | The entity passes shared model, adapter, access, action, and presentation contracts |

The absence of a required view, test category, named adapter, or evidence link means Phase 1 is
incomplete even if a generic schema exists.

Phase 1 may reuse and extract logic from existing modal components, but it must not treat those modals as the canonical entity implementation.

---

# PHASE 2 — Replace and Validate Existing Views

Phase 2 begins only after the relevant entity’s Phase 1 model is complete.

Its purpose is to replace every scattered representation with a view derived from the canonical entity model.

## Phase 2.1 Inventory every existing view

For each entity, identify:

- pages;
- table rows;
- list items;
- cards;
- dashboard widgets;
- search results;
- forms;
- inline detail panels;
- drawers;
- popovers;
- modals;
- references embedded inside other entities.

Record for every location:

- API response shape;
- current fields;
- current actions;
- viewer types;
- viewing context;
- missing or excessive information;
- whether hydration is necessary.

## Phase 2.2 Define derived views

Create only the views the application actually needs:

- `Summary`
- `Row`
- `Card`
- `Details`
- `Editor`
- specialized workflow view where justified

These views consume:

- canonical entity model;
- access result;
- context;
- optional action handlers.

They do not normalize raw API data or calculate permissions.

## Phase 2.3 Replacement order for each entity

For each entity:

1. Replace the richest existing detail representation.
2. Replace full pages.
3. Replace management and moderation views.
4. Replace cards and table rows.
5. Replace search and analytics projections.
6. Replace embedded references inside other entities.
7. Rebuild modals and drawers as thin containers around canonical views.
8. Remove old local formatting, access checks, and field mappings.

## Phase 2.4 Partial data and hydration validation

For each view:

- confirm the adapter matches its endpoint;
- confirm unloaded fields are not shown as empty;
- confirm detailed views hydrate only when needed;
- confirm hydration is authorized;
- confirm partial search/analytics data remains usable without forced hydration;
- confirm related entities use their own canonical summaries.

## Phase 2.5 Permission validation

Validate each view against:

- guest;
- ordinary authenticated User;
- entity owner;
- Vendor owner;
- participating Order Vendor;
- Admin with only the minimum relevant permission;
- Admin without the permission;
- User who is both Vendor and Admin;
- Super Admin.

Verify both:

- what information is shown;
- what actions are available.

## Phase 2.6 Presentation-container validation

Pages, modals, drawers, and popovers must be thin containers.

They may control:

- navigation;
- overlay and focus behavior;
- size and responsive layout;
- open/close/back controls.

They must not define:

- entity fields;
- DTO normalization;
- permissions;
- ownership;
- status meaning;
- legal entity actions.

## Phase 2.7 Final repository cleanup

After replacing an entity:

- mark every inventory location as migrated, removed, or approved exception;
- search for direct raw field rendering outside its entity/projection/command domain;
- remove duplicated badges, status maps, formatters, aliases, and ownership checks;
- remove page-local and modal-local permission interpretation;
- move modal-specific semantic sections into the entity domain;
- verify containers perform no DTO normalization or domain hydration;
- test all required viewer/context combinations in actual production views;
- run production build, all entity tests, integration tests, and browser checks;
- attach command output and file links to the compliance ledger.

Phase 2 for an entity is incomplete if even one production representation is absent from the
inventory, still consumes a raw payload, duplicates domain logic, or lacks viewer/context evidence.

---

# Phase 2 migration priority

Use the same dependency order as Phase 1:

1. User/account facets
2. Vendor
3. Product
4. Order
5. Gift Flow
6. Cart
7. Review
8. Category
9. Vendor Application
10. Commission
11. Commission Payment Request
12. Commission Rule
13. Report
14. Admin Request
15. Order Assistance Request
16. Notification
17. Vendor Feedback
18. Delivery Zone
19. Vendor Delivery Pricing
20. Reminder
21. Vendor Activity
22. User Review Restriction
23. Favorite relationship

An entity can enter Phase 2 as soon as its own Phase 1 model and any required dependency models are complete.

---

# Audited Backend Contract Registry

This registry closes the meaning of “all fields” and “all known shapes” for the baseline commit.
Because this specification is immutable during implementation, backend drift is recorded in
`BASELINE.md` and `BACKEND_BLOCKERS.md`. If the drift changes required scope, the implementation
remains `FRONTEND_INCOMPLETE` until the user approves a revision of this specification.

| UI domain | Authoritative backend sources | Consumed delivery shapes | Backend operations/states to mirror |
| --- | --- | --- | --- |
| User | `User`, `Address`, linked `Admin`, linked `Vendor`, `UserReviewRestriction` | User domain, `/users/me`, `PublicUserProfileResponse`, `AuthResponse.UserInfo`, admin User rows, analytics customer references, Order customer snapshots | update profile/addresses, request Admin, ban, unban, delete, facet operations |
| Vendor | `Vendor` | Vendor domain, verified public list, `/vendors/me`, unified search Vendor result, platform/vendor analytics references | update, verify, deactivate |
| Product | `Product`, `ProductDetails`, `ProductImage`, Category relation | Product domain, `ProductSearchResponse`, unified search Product result, recommendation Product, platform/vendor analytics references, Order/Cart snapshots | create/update/delete, submit, approve, reject, enable/disable, stock, images, categories, discount |
| Order | `Order`, `OrderItem`, `GuestInfo` | Order domain pages/lists, customer/vendor/admin pages, security projection, analytics references | place, pay, ship, deliver, cancel, refund/status override, estimate/delay, commission state |
| Gift Flow | `GiftFlow` | `GiftFlowResponse`, unified search Gift Flow result, favorite reference | create, update, delete; parsed configuration and Product constraints |
| Cart | `Cart`, `CartItem` | `CartResponse` and local guest-cart shape | add/update/remove item, remove group, clear |
| Review | `Review` | `ReviewResponse` public/self/moderation lists | create, approve, reject; anonymity and moderation state |
| Category | `Category` | Category domain/list and Product embedded relation | create/manage/delete; normalize `id/categoryId` and `name/categoryName` |
| Vendor Application | `VendorApplication` | `VendorApplicationResponse`, submit command | submit, approve, reject |
| Commission | `Commission` | `CommissionDTO` owner/admin lists | payment submitted, paid, overdue, urge, submit proof |
| Commission Payment Request | `CommissionPaymentRequest` | `CommissionPaymentRequestDTO` | submit, approve, reject |
| Commission Rule | `CommissionRule` | `CommissionRuleDTO` | create global/vendor rule, deactivate |
| Report | `Report` | Report domain pages | submit, under review, action taken, dismiss, resolve |
| Admin Request | `AdminRequest` | `AdminRequestDTO` self/admin/User relation | submit, approve, reject, invalidate, reset cooldown |
| Order Assistance | `OrderAssistanceRequest`, `OrderAssistanceMessage` | request/message DTOs | request, reply, in progress, resolve, reopen/confirm, close |
| Notification | `Notification` | owner notification list/count | mark read, mark all read; typed metadata reference |
| Vendor Feedback | `VendorFeedback` | `VendorFeedbackResponse` | submit, approve, reject; private moderation record |
| Delivery Zone | `DeliveryZone` | `DeliveryZoneResponse` | active checkout reference; activate/deactivate if exposed |
| Vendor Delivery Pricing | `VendorDeliveryPricing`, composite ID | `VendorDeliveryPricingResponse` | owner update |
| Reminder | `Reminder` | Reminder domain | create, process, owner delete |
| Vendor Activity | `VendorActivity` | `VendorActivityResponse` | owner read; typed metadata/reference |
| User Review Restriction | `UserReviewRestriction` | `UserReviewRestrictionResponse` | create/update/remove; expiry/active derivation |
| Favorite | `FavoriteProduct` | Favorite domain relation | add/remove Product or Gift Flow target; exactly one target |

## Normative capability map

This map defines frontend presentation capabilities against the current audited backend. Exact
permissions are additive; one permission never implies another.

| Viewer capability | Domains and limits |
| --- | --- |
| Guest/Public | Approved/discoverable Products, verified Vendors, public Gift Flows, Categories, public User projection, approved non-private Review content, active Delivery Zones, public search/recommendations |
| Authenticated User | Public access plus own User profile/addresses, own Cart, own Orders, own Notifications, own Reminders, own Favorites, own Admin Requests, own Vendor Applications, own Reviews, report/review/feedback submission |
| Vendor owner | Authenticated User access plus own Vendor management, own Products and discounts, own Gift Flows, own Delivery Pricing/analytics/activity, own Commissions/payment requests, participating Order Items and fulfilment data, own Order Assistance |
| `VIEW_USERS` | Protected User read fields only; does not grant mutation, Admin permissions, addresses requiring `MANAGE_USERS`, or unrelated domain access |
| `MANAGE_USERS` | Supported User metadata/address administration only; does not imply ban/delete/Admin mutation |
| `BAN_USERS` / `UNBAN_USERS` / `DELETE_USERS` | Only the named User action |
| `REVIEW_ADMIN_REQUESTS` | Read/reject/invalidate Admin Requests as currently supported |
| `MAKE_ADMINS` | Promote/approve applicable Admin Requests and reset cooldown where supported |
| `DEMOTE_ADMINS` | Demote Admin only |
| `MANAGE_ADMIN_PERMISSIONS` | View and mutate Admin permissions only |
| `MAKE_VENDORS` / `ACTIVATE_VENDORS` | Vendor Application review/read according to current endpoints; `ACTIVATE_VENDORS` also activates Vendor |
| `DEACTIVATE_VENDORS` | Vendor deactivation only |
| Product permissions | `ACTIVATE_PRODUCTS`, `REJECT_PRODUCTS`, `DEACTIVATE_PRODUCTS`, and `DELETE_PRODUCTS` remain independent; current backend mismatch is recorded below |
| `MANAGE_CATEGORIES` | Category mutation only |
| `VIEW_ORDERS` | Administrative Order read only |
| `MANAGE_ORDERS` | Supported broad Order operations only; does not substitute for exact newer permissions where the endpoint requires them |
| `MANAGE_ORDER_STATUS` | Admin status override only |
| `REVIEW_ORDER_ASSISTANCE` | Assistance read/reply/resolve/close through the current Commission assistance endpoints |
| Financial permissions | `VIEW_FINANCIAL_DATA`, `MANAGE_COMMISSIONS`, `REVIEW_COMMISSION_PAYMENTS`, `URGE_COMMISSION_PAYMENT`, and `VIEW_FINANCIAL_ANALYTICS` remain independent |
| `MANAGE_GIFT_FLOWS` | Administrative Gift Flow management where an endpoint exists; it does not create Vendor ownership |
| `SEND_NOTIFICATIONS` | Compose/send notifications; never read another User's notifications |
| `MANAGE_REPORTS` | Report administration |
| `VIEW_REVIEWS` | Protected Review/restriction read only |
| `MODERATE_REVIEWS` | Review approve/reject only |
| `VIEW_VENDOR_FEEDBACK` | Current Vendor Feedback read and backend-supported moderation; mismatch is recorded below |
| `MUTE_USERS` | User Review Restriction mutation only |
| Super Admin | All permissions plus any simultaneous User and Vendor ownership; still limited by loaded data and context |

When an endpoint and this intended capability vocabulary disagree, the Backend Contract Register
controls certification. The frontend may not broaden access to make the disagreement convenient.

## Normative canonical field catalog

These names are the canonical frontend vocabulary. Delivery aliases are accepted only by named
adapters. Derived/enriched fields must be marked as such in the schema.

| UI domain | Intrinsic canonical fields |
| --- | --- |
| User | `id`, `email`, `fullName`, `phoneNumber`, `birthday`, `addresses`, `isBanned`, `requestedAdmin`, optional projection `memberSince`; facets `vendor`, `admin`, `reviewRestriction` |
| User Vendor facet | `isVendor`, `supplierId`, optional canonical Vendor reference |
| User Admin facet | `isAdmin`, `permissions`, `isSuperAdmin`, derived `isCommunityHelper` |
| Vendor | `userId`, `supplierId`, `storeName`, `description`, `logoUrl`, `bannerUrl`, `contactEmail`, `contactPhone`, `address`, `websiteUrl`, `instagramUrl`, `facebookUrl`, `workingHours`, `isVerified` |
| Product | `id`, `supplierId`, `name`, `description`, `price`, `details`, `status`, `createdAt`, `updatedAt`, `publishedAt`, `averageRating`, `reviewCount`, `stockQuantity`, `discountPercentage`, `discountStartDate`, `discountEndDate`, `categories`, `images` |
| Order | `id`, `customerId`, `guestInfo`, `status`, `items`, `totalAmount`, `placedAt`, `shippingAddress`, `paymentMethod`, `customerName`, `customerEmail`, `instapayPhoneNumber`, `deliveryZoneId`, `deliveryCost`, parsed/raw `deliveryCostBreakdown`, `estimatedDeliveryDate`, `actualDeliveryDate`, `deliveryNotes`, `commissionPaid`, `commissionPaidAt` |
| Gift Flow | `id`, `supplierId`, `name`, `description`, raw `configuration`, `imageUrl`, `createdAt`, `updatedAt`; derived parsed configuration, normalized steps, Product references, and constraints |
| Cart | `id`, `customerId`, `items`, projection `total`, `updatedAt` when supplied |
| Review | `id`, `userId`, `reviewType`, `entityId`, `rating`, `comment`, `status`, `createdAt`, `reviewedAt`, `reviewedBy`, `moderatorNotes`, `isAnonymous`, `contentScore`, `orderId` |
| Category | canonical `id`, canonical `name`, optional Product references/count; backend `categoryId` and `categoryName` are aliases only |
| Vendor Application | `id`, `userId`, `storeName`, `description`, `logoUrl`, `bannerUrl`, `contactEmail`, `contactPhone`, `address`, `websiteUrl`, `instagramUrl`, `facebookUrl`, `workingHours`, `status`, `submittedAt`, `reviewedAt`, `reviewedBy`, `rejectionReason` |
| Commission | `id`, `orderId`, `supplierId`, `orderSubtotal`, `commissionRate`, `commissionAmount`, `status`, `dueDate`, `paidAt`, `createdAt`; enrichment `supplierName`; derived `overdue` |
| Commission Payment Request | `id`, `commissionId`, `supplierId`, `message`, `proofImageUrl`, `status`, `submittedAt`, `reviewedAt`, `reviewedBy`, `rejectionReason`; enrichment `supplierName` |
| Commission Rule | `id`, `type`, `supplierId`, `rate`, `startDate`, `endDate`, `active`, `createdAt`, `createdBy`; enrichment `supplierName` |
| Report | `id`, `reporterId`, `reportType`, `reportedEntityId`, `reason`, `description`, `status`, `createdAt`, `reviewedAt`, `reviewedBy`, `adminNotes` |
| Admin Request | `id`, `userId`, `message`, `status`, `requestedAt`, `reviewedAt`, `reviewedBy`, `reviewNotes`, `canReapplyAt`; enrichments `userEmail`, `userFullName` |
| Order Assistance | `id`, `orderId`, `supplierId`, `message`, `status`, `requestedAt`, `resolvedAt`, `resolvedBy`, `resolution`, `messages`; enrichment `supplierName` |
| Notification | `id`, `userId`, `title`, `message`, `type`, `read`, `createdAt`, raw/parsed `metadata`, typed derived `relatedEntity` |
| Vendor Feedback | `id`, `userId`, `vendorId`, `orderId`, `feedback`, `status`, `createdAt`, `reviewedAt`, `reviewedBy`, `moderatorNotes`, `contentScore` |
| Delivery Zone | `id`, `zoneName`, `description`, `isActive` |
| Vendor Delivery Pricing | `vendorId`, `zoneId`, `deliveryCost`, `updatedAt`; enrichment `zoneName` |
| Reminder | `id`, `customerId`, `description`, `scheduledAt`, `processed` |
| Vendor Activity | `id`, `vendorId`, `activityType`, `description`, `relatedEntityId`, raw/parsed `metadata`, `occurredAt` |
| User Review Restriction | `userId`, `canComment`, `canReview`, `restrictedAt`, `restrictedBy`, `reason`, `expiresAt`; derived `isActive` |
| Favorite | `id`, `userId`, `productId`, `flowId`, `addedAt`; exactly one typed target reference |

Sensitive and system fields remain part of the canonical vocabulary even when most payloads leave
them unloaded or access marks them forbidden. `passwordHash` is the sole backend User field that is
explicitly prohibited from all frontend models.

## Normative enum and state catalog

The frontend must import these from centralized schema constants and must never repeat string
literals or status-style maps in pages:

- Product status: `DRAFT`, `PENDING_APPROVAL`, `APPROVED`, `REJECTED`, `DISABLED`.
- Order status: `PENDING`, `PAID`, `SHIPPED`, `DELIVERED`, `CANCELLED`, `REFUNDED`.
- Review/Vendor Feedback status: `APPROVED`, `PENDING_REVIEW`, `REJECTED`.
- Review type: `PRODUCT`, `VENDOR`.
- Vendor Application status: `PENDING`, `APPROVED`, `REJECTED`.
- Admin Request status: `PENDING`, `APPROVED`, `REJECTED`, `INVALIDATED`.
- Commission status: `PENDING`, `PAYMENT_SUBMITTED`, `PAID`, `OVERDUE`.
- Payment Request status: `PENDING`, `APPROVED`, `REJECTED`.
- Commission Rule type: `GLOBAL`, `SUPPLIER_SPECIFIC`.
- Assistance status: `PENDING`, `IN_PROGRESS`, `RESOLVED`, `CLOSED`.
- Assistance sender: `VENDOR`, `ADMIN`.
- Report status: `PENDING`, `UNDER_REVIEW`, `ACTION_TAKEN`, `DISMISSED`, `RESOLVED`.
- Report type: `PRODUCT`, `GIFT_FLOW`, `USER`, `VENDOR`, `ADMIN`.
- Notification type: `ORDER_STATUS_UPDATE`, `PROMOTION`, `REMINDER`, `VENDOR_ALERT`,
  `SYSTEM_ALERT`, `REVIEW_REQUEST`, `DELIVERY_DELAY`, `DELIVERY_ESTIMATE_UPDATE`.
- Product target gender: `MALE`, `FEMALE`, `UNISEX`, `CHILD`.
- Vendor Activity type: `ORDER_RECEIVED`, `ORDER_SHIPPED`, `ORDER_DELIVERED`,
  `ORDER_CANCELLED`, `PRODUCT_CREATED`, `PRODUCT_UPDATED`, `PRODUCT_APPROVED`,
  `PRODUCT_REJECTED`, `PRODUCT_STOCK_UPDATED`, `PRODUCT_OUT_OF_STOCK`, `REVIEW_RECEIVED`,
  `FEEDBACK_RECEIVED`, `DELIVERY_PRICING_UPDATED`, `GIFT_FLOW_CREATED`, `GIFT_FLOW_UPDATED`.

Enum parity tests must fail when the backend adds or removes a value.

## Required semantic presentation matrix

`Details` means semantic entity details, not necessarily a modal. A dash means the inventory does not
currently require that variant.

| Entity | Summary/Card/Row | Details | Editor/workflow | Required embedded summaries |
| --- | --- | --- | --- | --- |
| User | Summary, admin row, customer reference | Public, self, protected/admin | self profile and Admin-permission workflow | Vendor facet, addresses, requests, restrictions |
| Vendor | storefront card, admin row, reference | public, owner, admin | Vendor settings | Product, Gift Flow, analytics references |
| Product | commerce card, search row, admin/vendor row, snapshot | public, owner, moderation | create/edit and discount workflow | Category, Vendor, Review |
| Order | history card/row, admin/vendor row | customer, Vendor-partial, admin | status, delivery, cancellation | User/Guest, Product snapshot, assistance, commission |
| Gift Flow | public/owner card, search reference | public, owner, admin | flow builder | Vendor and Product references |
| Cart | item/group summary | cart details | quantity/remove/checkout workflow | Product/Vendor snapshots |
| Review | public/self/moderation summary | public/self/moderation | submission/moderation commands | User and typed target references |
| Category | chip/reference, admin row | admin/public relation details | manage command | Product references/count |
| Vendor Application | self/admin summary | self/admin | submit/review commands | User reference |
| Commission | Vendor/admin row and summary | owner/admin financial | payment/urge workflow | Vendor and Order references |
| Commission Payment Request | owner/admin summary | owner/admin protected | proof/review workflow | Commission and Vendor references |
| Commission Rule | admin summary/row | admin | create/deactivate workflow | optional Vendor reference |
| Report | owner/admin summary | admin | submit/review workflow | reporter and typed target references |
| Admin Request | self/admin summary | self/admin | submit/review/cooldown workflow | User/reviewer references |
| Order Assistance | Vendor/admin summary | threaded details | reply/resolve/close workflow | Order, Vendor, messages |
| Notification | bell/page summary | owner details | read actions | typed related entity |
| Vendor Feedback | moderation summary | protected moderation | submit/moderate commands | User, Vendor, Order |
| Delivery Zone | checkout option/admin summary | only if management UI exists | selection/management | — |
| Vendor Delivery Pricing | pricing row | owner management | editor | Vendor and Zone |
| Reminder | dashboard summary | — | create/delete | User owner |
| Vendor Activity | timeline item | optional event details | — | typed related entity |
| User Review Restriction | protected summary | protected details | moderation workflow | User/Admin references |
| Favorite | decoration/remove control | — | add/remove | Product or Gift Flow |

The implementation may add a useful view, but it may not remove a required variant without updating
the inventory and obtaining explicit approval.

## Exhaustive embedded field requirements

The parent schema must include every field below:

- Address: `label`, `street`, `city`, `state`, `zipCode`, `country`, `isDefault`.
- Admin facet: `userId`, `permissions`, `isAdmin`, `isSuperAdmin`.
- Product Image: `id`, `productId`, `url`, `primary`, `displayOrder`.
- Order Item: `productId`, `productName`, `imageUrl`, `quantity`, `price`, `supplierId`,
  `groupId`, parsed/raw `metadata`.
- Guest Info: `email`, `firstName`, `lastName`, `phone`, `shippingAddress`.
- Cart Item response enrichment: Product snapshot, `quantity`, `groupId`, parsed/raw `metadata`,
  supplier reference, store-name enrichment, captured/display price.
- Order Assistance Message: `id`, `requestId`, `senderId`, `senderRole`, `message`, `createdAt`.
- Vendor Delivery Pricing identity: `vendorId` + `zoneId`.

Product Details must include every backend field in these groups:

- add-on prices and video;
- engraving, custom message, photo, embroidery, color, and size choices;
- wrapping, gift box, ribbon, and gift receipt;
- delivery dates, delivery range, perishability, and shelf life;
- recipient information requirements and anonymity;
- composition flags and item count;
- tags, featured/bestseller/new-arrival, gender, season, occasion, recipient type, and age group;
- slug, meta title, and meta description;
- Vendor SKU, Vendor notes, fulfilment time, handmade, made-to-order, and customizable.

No Product Details field may exist only in an upload/edit form or only in a modal.

# Projection and Command Registry

These contracts are mandatory because the UI consumes them even though they are not independent
entities.

| Contract | Required centralized model |
| --- | --- |
| Unified search | Product, Vendor, and Gift Flow partial references plus `totalResults` |
| Product search | Product partial model plus search-specific discount/in-stock derivations |
| Recommendations | Product canonical references plus `engine`, `strategy`, and `count` |
| Platform analytics | `TopProduct`, `TopCustomer`, and `TopVendor` metrics decorating canonical references |
| Vendor analytics | overview, Product performance, revenue periods, and Order-status breakdown |
| Financial analytics | totals, Vendor financial summaries, and monthly summaries |
| Authentication | token/session payload adapted once into canonical viewer and User reference |
| Product create/edit | Product command draft and payload mapper |
| Vendor profile/application | Vendor and Vendor Application command drafts |
| Gift Flow editor | Gift Flow configuration draft and validated payload mapper |
| Checkout | Order placement command and Cart-to-Order snapshot mapper |
| Commission proof/rule | payment-proof and Commission Rule command drafts |
| Report/review/feedback | submission command drafts separate from persisted entities |

Projection views may render projection metrics directly through projection selectors. They must use
canonical summaries for linked entities and may not create alternative Product/User/Vendor cards.

# Mandatory Production Migration Inventory

This list is the minimum known inventory from the audited frontend. Before implementation begins, a
fresh repository scan must add any newly introduced locations. A location may be checked off only
when it consumes canonical models and semantic views.

## User and account facets

- `Navbar.jsx`, `ProtectedRoute.jsx`, `authService.js`, and `useAuthStore.js` — canonical viewer/session.
- `PublicUserProfile.jsx` — public User details.
- `UserProfile.jsx` — self profile, addresses, Admin Requests, Vendor Applications.
- `AdminDashboard.jsx` — User rows/details, top-customer references, Admin management and permissions.
- `VendorOrders.jsx` and Order customer sections — protected customer reference.
- `Checkout.jsx` — self-profile command defaults, not a read representation.

## Vendor

- `VendorCatalog.jsx`, `VendorProfile.jsx`, `VendorSettings.jsx`.
- `GlobalSearch.jsx`, Home/recommendation Vendor references when present.
- `AdminDashboard.jsx` Vendor rows/details/top-Vendor analytics.
- Product, Gift Flow, Commission, Delivery Pricing, Order, and Vendor Application embedded Vendor
  references.

## Product and Category

- `HomePage.jsx`, `ProductCatalog.jsx`, `ProductDetails.jsx`.
- `ProductSearch.jsx`, `ProductRecommendations.jsx`, `GlobalSearch.jsx`.
- `VendorDashboard.jsx`, `VendorAnalytics.jsx`, `VendorProfile.jsx`.
- `Cart.jsx`, `Checkout.jsx`, `Favorites.jsx`.
- `Orders.jsx`, `OrderDetails.jsx`, `VendorOrders.jsx`, Order Item sections.
- `GiftFlowStep.jsx`, `GiftFlowCatalog.jsx`, `VendorGiftFlows.jsx`, Gift Flow Product sections.
- `AdminDashboard.jsx`.
- `UploadProduct.jsx` and `EditProduct.jsx` as command-draft exceptions using canonical schema/enums.

## Order, Cart, Delivery, and Assistance

- `Cart.jsx`, `useCartStore.js`, Navbar Cart indicator.
- `Checkout.jsx`, `ZoneSelector.jsx`.
- `Orders.jsx`, `OrderDetails.jsx`, `UserDashboard.jsx`.
- `VendorOrders.jsx`, Delivery Estimate and delay workflows.
- `AdminDashboard.jsx` Order and assistance panels.
- `OrderModal.jsx` and all current Order modal sections.
- `VendorDeliveryPricing.jsx` and Delivery Zone/Pricing views.

## Gift Flow

- `HomePage.jsx`, `GiftFlowCatalog.jsx`, `GiftFlowStep.jsx`.
- `Favorites.jsx`, `ProductDetails.jsx`, `VendorProfile.jsx`.
- `VendorGiftFlows.jsx`, `GlobalSearch.jsx`, Admin Gift Flow references.
- `GiftFlowModal.jsx` and all current Gift Flow modal sections.

## Review, Vendor Feedback, and restriction

- `ReviewList.jsx`, `ReviewForm.jsx`, `ProductDetails.jsx`, `VendorProfile.jsx`.
- `MyReviews.jsx`, `ModeratorReviews.jsx`.
- `ReviewModal.jsx` and its sections.
- `VendorFeedbackModal.jsx` command form and moderation representations.
- User restriction views in `MyReviews.jsx`, User details, and moderation workflows.

## Applications, requests, reports, and notifications

- Vendor Application: `BecomeVendor.jsx`, `MyVendorApplications.jsx`, `UserProfile.jsx`,
  `AdminVendorApplications.jsx`, `VendorApplicationModal.jsx`.
- Admin Request: `UserProfile.jsx`, `AdminDashboard.jsx`, User protected details,
  `AdminRequestModal.jsx`.
- Report: `ReportButton.jsx`, `AdminReports.jsx`, and typed target references.
- Notification: `NotificationBell.jsx`, `Notifications.jsx`, `NotificationModal.jsx`,
  Admin notification composition.

## Financial and supporting entities

- Commission, Payment Request, and Rule: `VendorCommissions.jsx`, `AdminFinancial.jsx`,
  `AdminDashboard.jsx`, `VendorOrders.jsx`, `CommissionModal.jsx`.
- Reminder: `UserDashboard.jsx` and notification-related references.
- Vendor Activity: `VendorActivityDashboard.jsx`.
- Favorite: `Favorites.jsx`, Product/Gift Flow favorite controls and decorations.

## Inventory record required for every location

Every inventory row in the implementation ledger must record:

```text
entity
file and component
route or parent view
service method / endpoint
named adapter
partial or complete source
required field paths
viewer types
context
semantic view used
hydration owner
actions
status: NOT_STARTED | IN_PROGRESS | MIGRATED | EXCEPTION
evidence
```

No statement that “all views were migrated” is valid without this ledger.

# Explicit Exceptions

The following exceptions are allowed only in the stated form:

1. **Command forms:** create/edit forms may hold raw draft values. They must use centralized command
   schemas, enums, validation, and payload mappers.
2. **Historical snapshots:** Order Items and Cart response items may display captured Product names,
   images, and prices. They must use a named snapshot adapter and must not be silently replaced by
   current Product values.
3. **Analytics metrics:** projection-specific metrics may remain in projection views, but linked
   entities use canonical references.
4. **Route guards:** route configuration may declare required capabilities, but capability
   interpretation must use the shared viewer helper.
5. **Submission modals:** report, review, feedback, payment-proof, and similar command dialogs are
   command containers, not entity details views.
6. **Generic primitives:** shared `Field`, `Section`, status-pill, date, and money primitives are
   allowed. Generic object dumping is not.

Every exception must be listed in the Migration Inventory. “Legacy,” “temporary,” or “works today”
is not an exception.

The six exception categories above are pre-approved only within their stated boundaries. The
implementation agent may document concrete instances of them, but may not create a new exception
category or expand an exception to avoid migration. Any new or expanded exception requires explicit
user approval and keeps the affected ledger row `FRONTEND_INCOMPLETE` until approved.

# Mechanical Repository Acceptance Checks

The final implementation must provide saved command output for these checks.

## Must have zero unapproved results

- raw `permissions.includes(...)`, local `hasPermission`, or local permission-set construction outside
  the shared viewer/access infrastructure;
- construction of ad-hoc viewer objects in pages or modals;
- DTO alias fallbacks in views, including `categoryName || name`, `productName || name`,
  `verified || isVerified`, and similar patterns;
- entity-specific `new Date`, `toLocale*`, `toFixed`, money, rating, ID, or status formatting outside
  entity/projection selectors and shared formatting primitives;
- service/API imports inside semantic entity presentations;
- adapter calls inside semantic presentations or modal shells;
- locally implemented entity-specific access/action decisions inside pages, modal shells, or
  sections; invoking the canonical builders from a controller is allowed;
- direct rendering of protected fields without field-state/access readers;
- debug logging of tokens, complete session/User objects, protected entity payloads, payment proof,
  addresses, or other sensitive fields;
- `JSON.stringify` used as a user-facing entity representation;
- invented defaults for unloaded backend values;
- duplicated entity Summary/Card/Details markup outside the owning entity domain;
- superseded entity sections remaining under `components/modals/<entity>`;
- one-request-per-row hydration introduced by entity views.

Results belonging to an explicit exception must appear in `EXCEPTIONS.md` with the exact file,
symbol, exception category, reason, reviewer, approval date, and removal condition when temporary.
The implementation agent cannot list itself as the approver of a new or expanded exception. An
allowlist cannot hide a requirement that the plan says must be centralized.

## Must have positive evidence

- exactly 23 registered top-level entity schemas;
- all mandatory embedded schemas;
- all named source adapters from the Backend Contract Registry;
- all migration-inventory locations;
- centralized viewer usage at every protected route and access calculation;
- semantic views imported by production pages;
- `UI_QUALITY.md` evidence for every required semantic view variant;
- tests for every entity and projection contract.

# Required Test Matrix

## Per-entity automated tests

Every entity must test:

1. full domain payload;
2. every named partial source;
3. alias normalization;
4. unknown-field rejection/containment;
5. idempotent adaptation;
6. loaded-field metadata;
7. available/empty/unloaded/invalid/forbidden behavior;
8. merge/hydration behavior;
9. owner and unrelated viewer;
10. exact permission and unrelated permission;
11. User+Vendor+Admin additive viewer;
12. Super Admin;
13. every relevant context;
14. every backend state and legal action;
15. absent handler suppresses action;
16. semantic view rendering for partial and complete models;
17. protected fields absent from rendered output when forbidden.

## Shared viewer matrix

All protected domains must be tested against:

- Guest;
- authenticated ordinary User;
- entity owner/User self;
- unrelated User;
- Vendor owner;
- unrelated Vendor;
- participating Order Vendor;
- Admin with exactly the required permission;
- Admin with an unrelated permission;
- User who is simultaneously Vendor and limited Admin;
- Super Admin who is also a Vendor;
- banned User where routing/session behavior applies.

## Browser verification

At least one actual application route or embedded production component for every required view
variant must be browser-tested against a local, staging, or otherwise explicitly approved non-live
environment. A synthetic component harness may supplement this verification but cannot replace all
real-route checks.

`VERIFICATION.md` must define the non-secret fixture accounts and records used for each viewer:
ordinary User, owner, unrelated User, Vendor, unrelated Vendor, participating Order Vendor, exact-
permission Admin, unrelated-permission Admin, Vendor+Admin, Super Admin, and banned User. The record
must identify how fixtures were created or reset without containing passwords, tokens, payment proof,
personal addresses, or other secrets.

Browser evidence must cover both visible information and absent information. Screenshots alone are
insufficient; the verification record must state the route/component, baseline commit, fixture,
viewer, context, loaded source, visible sections, hidden sections, actions, and assertion result.
Destructive actions must run only against disposable test data.

The same browser pass must verify the Professional UI quality contract, including keyboard/focus
behavior, required view states, and the minimum mobile/tablet/desktop viewport set. Results are
recorded in `UI_QUALITY.md`.

If the required environment, fixtures, or authorization to test is unavailable, the affected browser
cell remains `FRONTEND_INCOMPLETE`; unit tests are not a substitute.

Build and unit-test success are necessary but not sufficient.

# Entity Compliance Ledger Template

Implementation must copy this template into
`docs/entity-ui-unification/COMPLIANCE_LEDGER.md` and maintain one row per entity there. The template
inside this specification remains unchanged.

| Entity | Schema/domain parity | Named adapters | Field states | Access/RBAC | Actions | Semantic views | Tests | Inventory migrated | Cleanup pass | Browser matrix | Final status |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| User/account facets |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Vendor |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Product |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Order |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Gift Flow |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Cart |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Review |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Category |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Vendor Application |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Commission |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Commission Payment Request |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Commission Rule |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Report |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Admin Request |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Order Assistance |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Notification |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Vendor Feedback |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Delivery Zone |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Vendor Delivery Pricing |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Reminder |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Vendor Activity |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| User Review Restriction |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |
| Favorite relationship |  |  |  |  |  |  |  |  |  |  | `FRONTEND_INCOMPLETE` |

A cell is complete only when it links to files, tests, migration rows, or command output. A checkmark
without evidence is treated as empty.

# Definition of 100% Frontend Complete

The frontend plan is 100% complete only when all statements below are true:

1. Every backend entity and embedded value has one exhaustive canonical frontend schema.
2. Every consumed response shape has one named adapter.
3. Every runtime field has reliable available/empty/unloaded/invalid/forbidden behavior.
4. One viewer model represents Guest, User, Vendor, Admin, combined facets, and Super Admin.
5. Every entity has centralized context, ownership, RBAC, selectors, and actions.
6. Every required semantic presentation lives in the entity domain.
7. Every production representation in the inventory uses those presentations or an approved
   command/projection exception.
8. Every migrated presentation satisfies the Professional UI quality contract with evidence.
9. Modals, pages, drawers, and popovers are thin containers.
10. No unapproved legacy normalization, formatting, permission, ownership, transition, or duplicated
   representation remains.
11. Every required automated and browser matrix passes.
12. Every backend contract mismatch is recorded. For each affected frontend behavior, the UI either
    truthfully follows the current backend contract, safely withholds the behavior, or marks the
    ledger cell blocked when neither is possible. Backend resolution is required for
    `SYSTEM_CERTIFIED`, not automatically for `FRONTEND_COMPLETE`.
13. A fresh adversarial audit finds zero unresolved issues.

If any item is unknown, inferred, deferred, untested, broken, or allowlisted without justification,
the correct frontend result is **not `FRONTEND_COMPLETE`**.

# Backend Contract Register

The frontend implementation must not silently compensate for backend authorization or response
problems. The following audited items require explicit resolution or documented backend decisions:

1. Product rejection currently checks `ACTIVATE_PRODUCTS` although `REJECT_PRODUCTS` exists.
2. Vendor Feedback approve/reject currently checks the read permission `VIEW_VENDOR_FEEDBACK`; the
   backend has no distinct mutation permission.
3. Some Order mutation endpoints use `@PostAuthorize` where authorization should occur before
   mutation; UI availability cannot prove backend safety.
4. Notification `markAsRead(id)` must prove the notification belongs to the current User.
5. Public Product endpoints return domain objects containing fields the UI treats as Vendor/admin
   data; backend projections should ultimately enforce least-data delivery.
6. Public Vendor delivery-pricing reads expose all Vendor pricing. Confirm that this is intentional.
7. Auth responses mix broad roles and fine-grained permissions in the authorities list; the canonical
   viewer must normalize this once.
8. User has no intrinsic `createdAt` field while the public projection exposes `memberSince`; the
   current service populates it with `LocalDate.now()`, not the account creation date. The frontend
   must not present it as truthful membership history until the backend supplies real data.
9. Duplicate/legacy endpoint families exist for Vendor application, Order assistance, and Favorites;
   adapter and service ownership must identify the supported contract.
10. `ProductDetails` contains `allowsEmbroidery` and `allowsPhotoUpload`, but the current Product
    create/update DTO does not expose them. It exposes `requiresDeliveryDate` and
    `allowsScheduledDelivery`, while the controller's mapping does not currently apply those values.
11. `isCommunityHelper` is a public projection defined by the current backend as “has an Admin record
    with at least one permission.” It is not an exclusive role and must not be independently inferred
    by arbitrary views. The named User projection/Admin adapters own this derivation.

An affected frontend model may still satisfy `FRONTEND_COMPLETE` when it accurately represents the
current contract and safely withholds behavior that cannot be trusted. The blocker remains in
`BACKEND_BLOCKERS.md` and prevents `SYSTEM_CERTIFIED`. If no safe and truthful frontend behavior is
possible, the affected frontend ledger cell remains blocked.

# Future Entity Governance

This plan remains enforceable after the migration:

- Adding a backend `@Entity`, embedded value, enum, DTO, endpoint, field, relation, or operation
  requires updating the Backend Contract Registry and the owning UI schema.
- Adding a new frontend representation requires a Migration Inventory row before merge.
- A backend response-shape change requires a named adapter and partial-field test.
- A permission or ownership change requires viewer/access/action matrix tests.
- A new status or transition requires selector, action, and UI tests.
- A new projection must decorate canonical entity references instead of forking entity identity.
- CI should fail when registered backend fields, permissions, enums, adapters, or inventory entries
  drift from the frontend registry.

The desired maintenance property is:

> A domain change is made once in the owning entity or projection contract, and every UI
> representation receives the change through centralized selectors, access, actions, and semantic
> views.

---

# Required Implementation Sequence

1. Record the active repository root, branch, HEAD commit, dirty worktree state, and audit timestamp in
   `BASELINE.md`. Do not switch branches, discard changes, or assume a previous branch contains the
   current work.
2. Freeze this specification. Do not edit it during implementation.
3. Generate the detailed Migration Inventory from current routes, pages, components, stores,
   services, entity imports, modal sections, raw field usage, and permission checks.
4. Reconcile the generated inventory with the minimum inventory in this document. Before broad
   migration begins, record the inventory baseline commit. Newly discovered locations must be added;
   existing rows may not be silently removed.
5. Populate `COMPLIANCE_LEDGER.md` with current evidence. Empty or assumed cells remain
   `FRONTEND_INCOMPLETE`.
6. Build/fix the shared model, viewer, access, field-state, relation, hydration, and action contracts.
7. Complete Phase 1 entity by entity in dependency order.
8. Complete projection and command contracts needed by that entity.
9. Migrate every production location for that entity.
10. Run entity-specific cleanup searches and tests, saving evidence in `VERIFICATION.md`.
11. Browser-test the required viewer/context matrix.
12. Repeat for all 23 domains.
13. Run repository-wide cleanup, full automated verification, production build, and regression checks.
14. Re-run inventory discovery against the final HEAD and reconcile it with the baseline. New routes,
    service methods, entity references, or raw representations introduced during implementation must
    be added and migrated before completion.
15. Perform a separate adversarial audit intended to disprove completion. It must be executed in a
    fresh agent/thread context or by another reviewer that reads this specification and the evidence
    artifacts without relying on the implementation conversation. Record reviewer identity/context,
    tested commit, searches, and every finding in `ADVERSARIAL_AUDIT.md`. The implementing context
    may fix findings but may not be the sole approver of its own completion.
16. Fix every finding and repeat the relevant verification and independent adversarial checks.
17. Report `FRONTEND_COMPLETE` only when the external ledger and all evidence gates are fully
    satisfied. Report `SYSTEM_CERTIFIED` only if backend work was separately authorized and its
    blockers were resolved and verified.

## Checkpoint and resume protocol

This task may span multiple agent contexts. At the end of every completed entity:

- update the external ledger and every affected inventory row;
- append exact verification evidence;
- update the relevant `UI_QUALITY.md` checks;
- record unresolved findings and the next concrete step;
- leave incomplete cells explicitly incomplete.

On every resumed context, the agent must re-read this specification, `BASELINE.md`,
`COMPLIANCE_LEDGER.md`, `MIGRATION_INVENTORY.md`, `BACKEND_BLOCKERS.md`, and the latest verification
and UI-quality results before changing code. Conversation summaries and previous claims are not
authoritative.
