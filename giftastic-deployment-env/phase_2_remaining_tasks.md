# Phase 2 Remaining Functional Tasks

## Purpose

This document is the closed implementation backlog for completing Phase 2 of
`ENTITY_UI_DOMAIN_UNIFICATION_PLAN.md`.

Phase 1 created the canonical entity, projection, command, Viewer, access, action, field-state, and
semantic-presentation architecture. Phase 2 is complete only when the production frontend actually
uses that architecture everywhere.

The required end state is:

> Every production API response, entity reference, representation, protected field read, legal
> action, mutation payload, mutation result, and hydration path is handled through the canonical
> entity/projection/command architecture. No old page-local or modal-local entity definition,
> normalization, permission interpretation, status meaning, formatting, or duplicated
> representation remains.

This document focuses on functional architectural completion. Browser matrices, visual regression,
formal evidence recording, and independent adversarial certification happen after these tasks.
Automated architectural tests remain in scope because they prevent incomplete migration and
regression.

## Implementation status — 2026-06-21

Tasks 1–5 are functionally complete on the current branch:

- Task 1: the production inventory is executable in
  `frontend/src/ui/entities/productionMigrationInventory.js`, with entity closure rows and flattened
  production-location evidence.
- Task 2: every inventory adapter is registered and backed by a named source contract; the
  architecture suite verifies canonical, partial-aware, idempotent adaptation.
- Task 3: login, registration, persisted sessions, refresh synchronization, and Admin-facet
  hydration all reconstruct the same additive User/Vendor/Admin/Super Admin Viewer.
- Task 4: reusable management, search, Cart, Product details, Order history, and Gift Flow
  execution/editor representations are owned by their entity domains. Page-local code is limited to
  workflow orchestration, command inputs, and page-specific layout.
- Task 5: Product, Order, Gift Flow, and Vendor semantic modal sections live under
  `frontend/src/ui/entities/<entity>/sections`; the legacy modal subdirectories contain no files.

Verification at completion: `npm run test:phase1` passes 261 tests and `npm run build` succeeds.
Tasks 6 onward remain the next implementation scope.

## Non-negotiable rules

1. The backend domain classes remain the source of truth for entity identity, fields, relations,
   states, and operations.
2. A raw DTO or API response must never be treated as a canonical model.
3. Every consumed response shape must use one explicitly named adapter or projection adapter.
4. Every production entity representation must use an entity-owned semantic presentation or a
   documented command/projection/workflow exception allowed by the main plan.
5. Pages and controllers may fetch, navigate, orchestrate workflows, hold transient form state, and
   supply action handlers. They may not redefine entity fields, permissions, ownership, status
   meaning, legal transitions, or reusable entity presentation.
6. Modals, drawers, and popovers are presentation containers. They may not become entity domains.
7. Viewer facets are additive. Every authenticated account is a User; Vendor, Admin, and Super Admin
   are additional facets that may coexist.
8. Context may reduce visible information but may never grant permission.
9. Loaded, empty, unloaded, invalid, and forbidden values must remain distinct.
10. A task is not complete because a canonical import was added. The old interpretation and
    duplicated representation must also be removed.
11. Existing functionality must be preserved unless the backend contract proves it invalid or unsafe.
12. Backend files are read-only for this work. Backend inconsistencies must be recorded, not silently
    compensated for in the frontend.

## What “Phase 2 functionally complete” means

Phase 2 reaches the QA/audit-only stage when all of the following are true:

- every current production consumer is present in the migration inventory;
- every service response consumed by the UI has a named entity or projection adapter;
- every persisted entity held by a page, component, or store is canonical;
- every entity representation is entity-owned or is an explicitly justified workflow/projection
  composition;
- every protected field is read through access and field-state-aware selectors/readers;
- every legal entity action is emitted by the owning action builder;
- every command form uses a centralized draft, validation, enum, and payload mapper;
- every mutation response or optimistic patch preserves canonical model metadata;
- every hydration path is authorized, controller-owned, cached or batched where repeated, and merged
  canonically;
- every related entity is represented through its own canonical summary/reference contract;
- page-local and modal-local entity formatters, status maps, ownership checks, permission checks, and
  transition logic have been removed;
- obsolete duplicate representations and modal-owned semantic sections have been removed;
- all 23 entity closure rows have no functional `NOT_STARTED`, `IN_PROGRESS`, `UNKNOWN`, or
  unjustified exception;
- mechanical architectural checks and automated tests pass.

At that point, only browser verification, visual/accessibility QA, evidence refresh, and independent
audit may remain.

---

# Execution strategy

## Work in dependency order

Use this order unless a concrete dependency requires a small adjustment:

1. Shared response/session/update infrastructure
2. User and account facets
3. Vendor
4. Product and Category
5. Order, Cart, Delivery, and Assistance
6. Gift Flow
7. Review, Vendor Feedback, and Review Restriction
8. Vendor Application, Admin Request, Report, and Notification
9. Commission, Commission Payment Request, and Commission Rule
10. Reminder, Vendor Activity, and Favorite
11. Cross-domain projections
12. Repository-wide cleanup and closure

## Required workflow for each production location

For every location being migrated:

1. Identify its service method and backend response shape.
2. Identify or create its named adapter/projection adapter.
3. Declare whether the source is complete or partial and its exact loaded fields.
4. Convert raw data at the response boundary.
5. Keep canonical data in component/store state.
6. Build access from the canonical Viewer, context, and explicit relationship facts.
7. Render an entity-owned semantic presentation.
8. Build legal actions through the entity action builder.
9. Route mutations through a command when payload construction is involved.
10. Adapt or canonically patch the mutation result.
11. Remove the superseded raw rendering, formatter, permission check, status check, and alias logic.
12. Add or update an architectural test proving the boundary remains canonical.

Do not migrate only the modal while leaving the row, card, table, analytics reference, or embedded
reference bespoke.

---

# Task 1 — Regenerate and close the production migration inventory

The current inventory is useful but remains summary-oriented. Create an exhaustive implementation
inventory from the current branch rather than assuming previous scans are complete.

## Required discovery scope

Scan:

- `frontend/src/App.jsx`
- `frontend/src/pages/**`
- `frontend/src/components/**`
- `frontend/src/store/**`
- `frontend/src/services/**`
- route guards and authentication/session boundaries;
- modal, drawer, popover, inline panel, table, card, row, list, search, and analytics consumers;
- create/edit/submission forms;
- embedded entity references;
- direct entity field reads;
- status, permission, role, ownership, and transition checks;
- mutation payload construction;
- mutation response handling;
- hydration and per-row fetching.

## Required row format

Every production location must record:

```text
entity or projection
file and component/function
route or parent consumer
service method and endpoint family
named adapter/projection adapter
complete or partial source
exact required canonical fields
Viewer types
viewing context
relationship/ownership inputs
semantic presentation
hydration owner
actions and action builder
command domain when applicable
mutation-result strategy
status: NOT_STARTED | IN_PROGRESS | MIGRATED | EXCEPTION
evidence
```

## Definition of done

- Every mandatory location from the main plan is represented.
- Every newly discovered location is added.
- No production location is represented only by a broad “page migrated” statement.
- No row remains `UNKNOWN`.
- Removing a location requires proof that the production consumer was deleted.

---

# Task 2 — Complete endpoint-to-adapter and projection coverage

Every consumed response shape must be converted at its service/controller/store boundary.

## Required work

- Trace every service method used by production UI.
- Associate every entity-bearing response with exactly one named adapter.
- Associate search, recommendation, authentication, and analytics responses with projection adapters.
- Add missing named source contracts for distinct list, details, self, public, moderation,
  management, analytics, or embedded response shapes.
- Avoid marking an endpoint complete merely because a generic domain adapter accepts its fields.
- Declare exact loaded fields and completeness for partial responses.
- Ensure mutation responses also use named adapters before entering UI state.
- Ensure adapters are idempotent when passed canonical models.
- Keep DTO aliases inside adapters only.
- Remove direct calls to generic entity adapters from production consumers when the source is known.
- Remove view-level alias interpretation such as `a || b` or `a ?? b` for backend field names.

## Current areas requiring explicit reconciliation

- User self/profile, public profile, Admin management, Order customer snapshot, and auth session.
- Vendor public list, self profile, Admin record, unified search, and analytics references.
- Product full domain, search, recommendation, analytics, Cart snapshot, and Order Item snapshot.
- Order customer list, Vendor list, Admin list, details, and security/analytics references.
- Gift Flow public/vendor responses, search references, and Favorite references.
- Vendor Application, Admin Request, Assistance, Commission, Payment Request, Rule, Report,
  Notification, Feedback, Delivery, Reminder, Activity, Restriction, and Favorite endpoint families.
- Duplicate/legacy endpoint families identified in the backend blocker register.

## Definition of done

- Every inventory row names an adapter or projection adapter.
- Every named adapter has a source contract and tests for exact loaded fields.
- No raw service response is stored or rendered as an entity.
- No known response uses an unnamed generic adaptation path.

---

# Task 3 — Finalize the canonical authentication session and Viewer boundary

There must be one stable application Viewer used by routes, navigation, pages, access builders, and
actions.

## Required work

- Normalize login, registration, persisted session, and session refresh through the authentication
  projection.
- Preserve the canonical User identity and loaded session fields.
- Preserve Vendor and Admin facets simultaneously.
- Preserve `SUPER_ADMIN` without removing User or Vendor ownership.
- Ensure persisted session data contains enough authoritative facet data to reconstruct the same
  Viewer after refresh.
- Ensure Admin profile hydration extends the existing Viewer instead of creating divergent,
  page-local authority state.
- Decide and centralize how refreshed Vendor/Admin facet data is persisted or invalidated.
- Ensure `Navbar`, `ProtectedRoute`, page controllers, and action/access builders consume the same
  Viewer object.
- Remove page-local role inference and ad-hoc Viewer construction.
- Ensure banned-state handling uses the canonical User field state rather than raw aliases.

## Current targets

- `frontend/src/services/authService.js`
- `frontend/src/store/useAuthStore.js`
- `frontend/src/components/ProtectedRoute.jsx`
- `frontend/src/components/Navbar.jsx`
- Admin profile hydration in Admin dashboard, reports, and moderation pages
- Vendor portal routing and navigation

## Definition of done

- Refreshing the application does not change recognized account facets unexpectedly.
- A combined User+Vendor+Admin Viewer remains combined everywhere.
- All protected routes and access calculations use the single application Viewer.
- No page maintains its own permission or role interpretation.

---

# Task 4 — Replace all remaining bespoke entity representations

Move every reusable entity representation into its owning entity domain.

## User/account targets

- Admin dashboard User management rows and protected contact/status rendering.
- Admin permission-management User references.
- Self profile identity and address representations where they are read views rather than form inputs.
- Protected Order customer references.
- Any remaining raw User labels in analytics, selectors, and notification composition.

## Vendor targets

- Admin dashboard Vendor rows and top-Vendor references.
- Embedded Vendor references in Product, Gift Flow, Commission, Delivery Pricing, Order, and Vendor
  Application surfaces.
- Vendor analytics references.
- Any raw public Vendor card/details representation not using `VendorSummary`/`VendorDetails`.

## Product and Category targets

- Vendor dashboard Product inventory rows.
- Admin dashboard Product rows and top-Product references.
- Reusable Product identity/details currently embedded in `ProductDetails`.
- Product references in Cart, Checkout, Orders, Gift Flow workflows, Favorites, analytics, and Vendor
  profile.
- Reusable Category chips/rows/details.

## Order, Cart, Delivery, and Assistance targets

- Vendor Order rows and status presentation.
- Admin Order rows.
- User dashboard Order representations.
- Reusable Cart Item and Cart Group views.
- Order Item historical snapshot presentation.
- Assistance list, thread, message, and status presentations.
- Delivery Zone and Vendor Delivery Pricing representations.

## Gift Flow targets

- Reusable public Flow details.
- Vendor Flow list/preview representation.
- Flow Product references.
- Flow execution header/summary elements that semantically represent the Flow.
- Favorite and search references.

## Moderation/application/support targets

- Vendor Application rows in self profile and Admin surfaces.
- Admin Request rows.
- Review moderation rows and target/author references.
- Vendor Feedback moderation rows.
- Report target references.
- Notification owner/system representations.
- User Review Restriction moderation/self representations.

## Financial and operational targets

- Commission, Payment Request, and Rule rows shared by Admin and Vendor financial surfaces.
- Vendor references in financial analytics.
- Reminder, Vendor Activity, and Favorite representations.

## Definition of done

- Repeated entity field markup exists only in the owning entity domain.
- Pages compose semantic views rather than reconstructing entity cards, rows, and details.
- Specialized workflow views are entity-owned when they encode entity semantics.
- Page-specific layout that does not define entity meaning may remain in the page.

---

# Task 5 — Move remaining modal-owned semantic sections into entity domains

The following directories still contain entity-specific semantic sections and must be reviewed:

- `frontend/src/components/modals/product/**`
- `frontend/src/components/modals/order/**`
- `frontend/src/components/modals/giftFlow/**`
- `frontend/src/components/modals/vendor/**`

## Required work

- Move field grouping, labels, status interpretation, protected-field gating, action presentation, and
  reusable details sections into `frontend/src/ui/entities/<entity>/**`.
- Make related entity sections compose the related entity's canonical summary.
- Keep modal shells limited to dialog behavior, layout, close/back controls, pending state, and
  composition.
- Delete superseded modal sections after imports are migrated.
- Keep truly generic dialog/layout primitives under shared modal components only if they contain no
  entity semantics.

## Definition of done

- Modal shells do not import services or adapters.
- Modal shells do not build access or actions.
- Modal subdirectories contain no reusable entity semantics.
- Changing an entity field, label, visibility rule, or formatting rule requires changing only the
  entity domain.

---

# Task 6 — Centralize all status, state, and derived-meaning interpretation

Pages may filter or arrange already-derived values, but they may not decide what a backend state
means.

## Known remaining examples

- Product approved, pending, and draft inventory counts in `VendorDashboard`.
- Payment Request pending counts in `VendorCommissions`.
- Product/Order status badges in management rows.
- Order transition options and temporary status objects.
- Vendor verification state.
- Application, Admin Request, Review, Report, Feedback, Assistance, and Notification status meaning.
- Stock availability and low-stock meaning.
- Commission overdue/payment state.
- Restriction active/expired state.

## Required destinations

- entity selectors for labels, styles, groups, counts, and derived state;
- action builders for legal transitions;
- projection selectors for projection metrics;
- command validation for draft-state rules.

## Definition of done

- Production pages contain no business decisions based directly on entity enum strings.
- Status label/style maps exist only in entity/projection domains.
- A new backend state requires one centralized update rather than edits across pages.

---

# Task 7 — Centralize all entity-specific formatting

Move remaining formatting into entity or projection selectors.

## Required categories

- `LocalDate` and `LocalDateTime`
- money and decimal values
- rates and percentages
- ratings
- identifiers and shortened IDs
- status labels and styles
- stock and availability labels
- address and contact formatting
- analytics metric formatting

## Required work

- Remove page/modal implementations of `new Date`, `toLocale*`, `Intl` entity formatting, and
  `toFixed` for entity/projection values.
- Remove one-off money suffixes and currency assumptions from entity representations.
- Centralize safe invalid/unloaded handling.
- Keep command serialization separate from display formatting.

## Definition of done

- Mechanical formatting scans have no unexplained entity-specific results outside entity/projection
  selectors and shared primitives.
- Invalid values cannot render as `Invalid Date`, `NaN`, or fabricated zero.

---

# Task 8 — Enforce protected-field and field-state-aware reading

A canonical model alone does not prevent accidental display. Protected reads must combine model
state, access, and context.

## Fields requiring focused review

- User email, phone, birthday, addresses, ban state, permissions, and restrictions.
- Vendor contact information, verification state, owner IDs, and system IDs.
- Product stock, SKU/vendor notes, moderation state, SEO/system data, and timestamps.
- Order customer/guest data, shipping address, payment data, delivery breakdown, Vendor
  participation, and financial totals.
- Vendor Application applicant/review data.
- Report and Review moderation fields.
- Vendor Feedback.
- Commission, Payment Request proof, and Rule fields.
- Notification ownership and metadata.
- Reminder, Favorite, Activity, and Restriction owner/system fields.

## Required work

- Replace broad `canRead` checks followed by unrestricted property access with field/section-aware
  selectors or readers.
- Ensure forbidden fields are not passed into child components as ordinary values.
- Ensure unloaded values are not represented as empty.
- Ensure detailed hydration cannot request forbidden fields.
- Ensure public contexts explicitly omit internal fields even when the Viewer owns or administers
  the entity.

## Definition of done

- Every protected field rendering path has an explicit access/field-state boundary.
- No direct protected-field read can bypass the owning entity policy.

---

# Task 9 — Complete canonical action-builder adoption

Every entity operation shown to a User must originate from the owning entity action builder.

## Required review areas

- User ban/unban, promotion/demotion, permission-related operations, and restriction changes.
- Vendor activation/deactivation.
- Product approve/reject/activate/deactivate/delete and Vendor-owned operations.
- Category operations.
- Order status, delivery estimate/delay, and Assistance operations.
- Gift Flow operations.
- Review, Vendor Feedback, Report, Vendor Application, and Admin Request moderation.
- Notification read actions.
- Commission, Payment Request, and Rule operations.
- Reminder deletion, Favorite removal, and Restriction operations.

## Required work

- Controllers supply service handlers to action builders.
- Pages render emitted actions or pass them to semantic views.
- Remove page-local permission plus status combinations that independently expose operations.
- Ensure absent handlers suppress actions.
- Ensure missing identity or unloaded state suppresses actions.
- Safely withhold actions affected by known backend permission mismatches.

## Definition of done

- A page cannot expose an operation the canonical action builder rejects.
- Legal transition rules exist in one entity-owned location.

---

# Task 10 — Complete command-domain adoption

Create/edit/submission forms may hold raw draft values, but backend payload semantics must be
centralized.

## Existing command domains to verify end to end

- Checkout
- Commission proof
- Commission Rule
- Delivery delay
- Delivery estimate
- Feedback submission
- Gift Flow editor
- Product create/edit
- Product discount
- Reminder
- Report submission
- Review Restriction
- Review submission
- Vendor Application
- Vendor profile
- Product search/filter command where applicable
- Vendor Delivery Pricing

## Missing or incomplete command candidates to close

- User profile editing.
- User address editing.
- Admin Request submission.
- Category create/update/delete payload handling where payload semantics exist.
- Admin notification composition.
- Any remaining moderation notes/reason payload built independently in pages.
- Any Gift Flow/cart metadata command still serialized manually outside a command mapper.

## Required work

- Centralize draft creation.
- Import canonical field names and enums.
- Centralize validation and error shape.
- Map only backend-supported payload fields.
- Prevent date, decimal, structured JSON, URL, and identity serialization from being duplicated in
  pages.
- Keep service invocation in controllers/pages after successful mapping.

## Definition of done

- No production form independently defines backend payload rules.
- The same workflow in multiple pages uses one command implementation.

---

# Task 11 — Normalize mutation responses and optimistic updates

Canonical model metadata must survive state changes.

## Required work

- Adapt successful mutation responses through the correct named adapter.
- Prefer authoritative mutation responses over guessed local patches.
- When optimistic/local patches are necessary, use canonical patch/merge helpers.
- Preserve identity, loaded fields, partial state, invalid fields, issues, relations, and source
  metadata.
- Update collections and selected-detail state through the same canonical operation.
- Rebuild Viewer/session facets centrally after User/Vendor/Admin mutations that affect the current
  account.
- Remove raw object spreads against canonical entities.
- Remove plain-object permission mutation where it represents canonical User/Admin facet state.

## Current focused targets

- Admin dashboard User/Admin permission mutations.
- Vendor and Product moderation.
- Order status updates.
- Vendor Orders.
- Review, Report, Application, Request, and Feedback moderation.
- Commission/payment/rule updates.
- Notification, Reminder, and Restriction mutations.
- Vendor profile/settings updates.

## Definition of done

- No mutation converts a canonical entity back into an ordinary object.
- Partial and field-state metadata remains truthful after every update.

---

# Task 12 — Finish hydration, caching, and relation ownership

Hydration must enrich canonical models without creating duplicate identity or N+1 behavior.

## Required work

- Declare required fields for detailed presentations.
- Authorize hydration before fetching.
- Keep hydration in pages/controllers/repositories, never semantic presentations.
- Merge hydrated results through canonical merge helpers.
- Cache or batch repeated Product, Vendor, User, Flow, or other relation hydration.
- Prevent list rendering from making one request per row.
- Preserve the partial model when hydration fails.
- Use entity references and canonical snapshots for embedded relations.
- Keep historical Order Item and Cart snapshots historical.
- Ensure Flow Product, Order customer, Product Vendor, analytics references, and report targets use
  related entity summaries rather than copied labels.

## Current focused targets

- Cart Product hydration cache.
- Gift Flow Product hydration.
- Favorites Product/Flow detail hydration.
- Product analytics detail hydration.
- Vendor profile Product/Flow relations.
- Order customer and Order Item references.
- Admin analytics User/Vendor/Product references.

## Definition of done

- Every hydration path has one owner and a clear cache/batch strategy.
- No semantic view fetches data.
- No related entity is silently redefined inside another entity's view.

---

# Task 13 — Complete projection boundaries

Projections may own workflow metrics but not alternative entity identity.

## Required projection review

- Authentication
- Unified Search
- Product Search
- Recommendations
- Platform Analytics
- Vendor Analytics
- Financial Analytics

## Required work

- Adapt each projection at its response boundary.
- Use projection selectors for metrics, ranking, pagination, and aggregate values.
- Adapt linked User, Vendor, Product, Order, or Gift Flow references canonically.
- Render linked identities through entity-owned summaries.
- Remove duplicate Product/Vendor/User cards from analytics and search screens.
- Keep hydration of partial references controller-owned.

## Definition of done

- Projection metrics remain projection-owned.
- Entity identity, visibility, formatting, and actions remain entity-owned.

---

# Task 14 — Consolidate duplicated cross-page workflows

Canonical entity components are not sufficient if the same controller/domain workflow is still
implemented independently in multiple pages.

## Known duplication to review

- Financial management in `AdminDashboard` and `AdminFinancial`.
- Commission Rule creation forms.
- Product moderation/list handling.
- Admin/Vendor Product detail hydration.
- Notification list and bell actions.
- Vendor Application self/Admin detail handling.
- Review moderation actions.
- Assistance conversation/action handling.

## Required work

- Extract shared hooks/controllers when two pages perform the same adaptation, access/action
  construction, mutation, and canonical update sequence.
- Keep page-specific layout separate from shared workflow ownership.
- Ensure shared workflow code consumes canonical models and commands.

## Definition of done

- A behavior rule or mutation sequence shared by multiple screens has one implementation.
- Pages do not drift in adaptation, authorization, or post-mutation state handling.

---

# Task 15 — Remove superseded and duplicate architecture

After replacement, remove the old code rather than leaving two implementations.

## Remove or consolidate

- duplicate Summary/Card/Row/Details markup;
- modal-owned entity sections;
- old DTO normalization utilities;
- legacy adapter aliases;
- local entity status maps and formatters;
- local permission and ownership helpers;
- local legal-transition helpers;
- duplicate command payload builders;
- raw canonical-entity object patches;
- dead hydration code;
- duplicate endpoint consumers no longer designated canonical;
- production routing to the Phase 1 harness;
- obsolete imports, props, and compatibility fallbacks.

## Definition of done

- There is one authoritative implementation for every entity rule and reusable representation.
- Searches for old patterns produce zero unexplained results.

---

# Task 16 — Add functional architectural enforcement

Tests must prevent the old architecture from returning before the browser/QA stage begins.

## Required mechanical checks

Fail when production code contains:

- raw permission-array interpretation;
- ad-hoc Viewer creation;
- DTO alias fallback in views;
- entity-specific formatting outside entity/projection selectors;
- adapters or services inside semantic presentations or modal shells;
- page-local ownership or legal-transition decisions;
- direct protected-field rendering without access/field-state handling;
- duplicated entity cards/details/rows outside entity domains;
- unsupported mutation payload construction;
- raw object-spread updates of canonical entities;
- modal-owned semantic sections;
- one-request-per-row hydration;
- unregistered service response shapes;
- inventory locations without a closure status.

## Required positive checks

- every inventory response names a registered adapter/projection;
- every required production representation imports an entity-owned semantic view;
- every protected route uses the canonical Viewer helper;
- every applicable mutation form uses a command mapper;
- every action-bearing representation uses an action builder;
- all 23 entity domains remain registered and contract-valid.

## Definition of done

- Architectural tests pass.
- Introducing a new raw representation or response boundary without registration fails CI/tests.

---

# Task 17 — Perform the final functional closure pass

This is mandatory. Tasks 1–16 describe known categories, but this pass proves no unknown functional
migration work remains.

## Per-location closure questions

For every production location:

1. Is every entity-bearing response adapted at the boundary?
2. Is canonical state retained through rendering and mutation?
3. Is the representation entity-owned?
4. Are related entities represented by their own domains?
5. Are partial/unloaded/invalid/forbidden fields handled correctly?
6. Is access derived from the canonical Viewer and context?
7. Are ownership/participation facts based on backend identifiers?
8. Are actions emitted by the owning action builder?
9. Are payload semantics owned by a command?
10. Are mutation results adapted or canonically patched?
11. Is hydration authorized, cached/batched, and controller-owned?
12. Has the superseded implementation been removed?

## Per-entity closure table

Create or update a table covering all 23 entities:

| Entity | Responses | Representations | Protected fields | Access/RBAC | Actions | Commands | Mutations | Hydration/relations | Legacy removed | Functional status |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| User/account facets |  |  |  |  |  |  |  |  |  |  |
| Vendor |  |  |  |  |  |  |  |  |  |  |
| Product |  |  |  |  |  |  |  |  |  |  |
| Order |  |  |  |  |  |  |  |  |  |  |
| Gift Flow |  |  |  |  |  |  |  |  |  |  |
| Cart |  |  |  |  |  |  |  |  |  |  |
| Review |  |  |  |  |  |  |  |  |  |  |
| Category |  |  |  |  |  |  |  |  |  |  |
| Vendor Application |  |  |  |  |  |  |  |  |  |  |
| Commission |  |  |  |  |  |  |  |  |  |  |
| Commission Payment Request |  |  |  |  |  |  |  |  |  |  |
| Commission Rule |  |  |  |  |  |  |  |  |  |  |
| Report |  |  |  |  |  |  |  |  |  |  |
| Admin Request |  |  |  |  |  |  |  |  |  |  |
| Order Assistance |  |  |  |  |  |  |  |  |  |  |
| Notification |  |  |  |  |  |  |  |  |  |  |
| Vendor Feedback |  |  |  |  |  |  |  |  |  |  |
| Delivery Zone |  |  |  |  |  |  |  |  |  |  |
| Vendor Delivery Pricing |  |  |  |  |  |  |  |  |  |  |
| Reminder |  |  |  |  |  |  |  |  |  |  |
| Vendor Activity |  |  |  |  |  |  |  |  |  |  |
| User Review Restriction |  |  |  |  |  |  |  |  |  |  |
| Favorite relationship |  |  |  |  |  |  |  |  |  |  |

Use only:

- `COMPLETE`
- `NOT_APPLICABLE` with a concrete reason
- `BLOCKED_BY_BACKEND` with safe frontend behavior documented
- `INCOMPLETE`

Do not use percentages or `MOSTLY_COMPLETE` in the closure table.

## Functional completion gate

Phase 2 may be handed to QA/audit only when:

- every applicable cell is `COMPLETE`;
- every `NOT_APPLICABLE` cell has a concrete architectural reason;
- every backend-blocked cell safely withholds unsupported behavior;
- there are no `INCOMPLETE` cells;
- there are no unknown production consumers;
- all architectural tests and the production build pass;
- the working tree contains no accidental backend changes.

---

# Current known implementation targets

The final inventory may discover more. At minimum, the working agent must explicitly inspect and
close these current targets:

## High-risk pages

- `frontend/src/pages/AdminDashboard.jsx`
- `frontend/src/pages/UserProfile.jsx`
- `frontend/src/pages/VendorDashboard.jsx`
- `frontend/src/pages/VendorOrders.jsx`
- `frontend/src/pages/VendorGiftFlows.jsx`
- `frontend/src/pages/ProductDetails.jsx`
- `frontend/src/pages/Cart.jsx`
- `frontend/src/pages/Checkout.jsx`
- `frontend/src/pages/GiftFlowStep.jsx`
- `frontend/src/pages/VendorAnalytics.jsx`
- `frontend/src/pages/AdminFinancial.jsx`
- `frontend/src/pages/ModeratorReviews.jsx`

## Cross-cutting components and stores

- `frontend/src/components/Navbar.jsx`
- `frontend/src/components/ProtectedRoute.jsx`
- `frontend/src/components/GlobalSearch.jsx`
- `frontend/src/components/NotificationBell.jsx`
- `frontend/src/components/ReviewList.jsx`
- `frontend/src/components/ProductSearch.jsx`
- `frontend/src/components/ProductRecommendations.jsx`
- `frontend/src/store/useAuthStore.js`
- `frontend/src/store/useCartStore.js`
- `frontend/src/services/authService.js`

## Modal semantic ownership

- `frontend/src/components/modals/product/**`
- `frontend/src/components/modals/order/**`
- `frontend/src/components/modals/giftFlow/**`
- `frontend/src/components/modals/vendor/**`

## Known direct-state logic

- Product status counts in `VendorDashboard`.
- Payment Request status count in `VendorCommissions`.
- User/Admin permission state updates in `AdminDashboard`.
- Session facet reconstruction and Admin facet hydration.
- Manual Gift Flow Cart metadata serialization.
- Repeated relation hydration in Favorites, Gift Flow, Product analytics, and Vendor profile flows.

---

# Work that belongs after functional completion

Do not use these later activities to hide incomplete migration. They begin after the functional gate:

- full real-route Viewer/RBAC browser matrix;
- visual regression and responsive verification;
- accessibility interaction QA;
- destructive workflow testing against disposable fixtures;
- evidence-document refresh;
- independent adversarial audit;
- final `FRONTEND_COMPLETE` certification.

If QA discovers raw payload use, duplicated entity semantics, local permission/transition logic,
missing commands, or non-canonical mutation updates, Phase 2 returns to implementation because those
are functional architectural defects.

---

# Functional closure result — 2026-06-21

Tasks 6–17 were re-executed against the production frontend. The table records functional
architecture only; browser QA, visual review, accessibility QA, and independent certification remain
the later work explicitly excluded above.

| Entity | Responses | Representations | Protected fields | Access/RBAC | Actions | Commands | Mutations | Hydration/relations | Legacy removed | Functional status |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| User/account facets | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Vendor | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Product | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Order | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Gift Flow | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Cart | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Review | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Category | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | BLOCKED_BY_BACKEND — update is not exposed; UI safely offers create/delete only | NOT_APPLICABLE — no related-entity hydration | COMPLETE | COMPLETE |
| Vendor Application | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Commission | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | NOT_APPLICABLE — no Commission create/edit form | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Commission Payment Request | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Commission Rule | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Report | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Admin Request | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Order Assistance | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Notification | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Vendor Feedback | COMPLETE | COMPLETE | COMPLETE | COMPLETE | BLOCKED_BY_BACKEND — backend exposes read permission only; unsupported moderation actions are withheld | COMPLETE | BLOCKED_BY_BACKEND — unsupported writes are not sent | NOT_APPLICABLE — response is self-contained | COMPLETE | COMPLETE |
| Delivery Zone | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | NOT_APPLICABLE — no production Zone mutation form | NOT_APPLICABLE — read/selection workflow only | NOT_APPLICABLE — response is self-contained | COMPLETE | COMPLETE |
| Vendor Delivery Pricing | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE |
| Reminder | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | NOT_APPLICABLE — response is self-contained | COMPLETE | COMPLETE |
| Vendor Activity | COMPLETE | COMPLETE | COMPLETE | COMPLETE | NOT_APPLICABLE — activity records expose no user operation | NOT_APPLICABLE — read-only domain | NOT_APPLICABLE — read-only domain | COMPLETE | COMPLETE | COMPLETE |
| User Review Restriction | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | NOT_APPLICABLE — response is self-contained | COMPLETE | COMPLETE |
| Favorite relationship | COMPLETE | COMPLETE | COMPLETE | COMPLETE | COMPLETE | NOT_APPLICABLE — add/remove endpoints have no request body | COMPLETE | COMPLETE | COMPLETE | COMPLETE |

There are no `INCOMPLETE` cells. Backend-blocked behavior is withheld rather than simulated.
