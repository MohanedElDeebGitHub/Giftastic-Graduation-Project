# Phase 2 Functional Completion Plan

## Objective

Finish Phase 2 at the point where no functional entity-unification work remains.

The required result is:

> Every production frontend consumer uses the canonical entity, projection, command, Viewer,
> access, action, selector, mutation, and hydration architecture. Pages and modal shells only
> coordinate workflows and layout; they do not redefine entity meaning.

This plan deliberately excludes:

- browser matrices;
- visual regression;
- responsive or accessibility QA;
- cosmetic cleanup;
- documentation polish;
- performance improvements that do not affect architectural correctness;
- independent certification.

Backend code must not be changed. Any backend limitation must be recorded and the unsupported UI
behavior safely withheld.

---

## Task 1 — Rebuild the production-consumer inventory

Create an exhaustive inventory from the current frontend rather than trusting previous completion
claims.

### Direction

Inspect all production files under:

- `frontend/src/pages`
- `frontend/src/components`
- `frontend/src/store`
- `frontend/src/services`
- authentication and route boundaries

Record every location that:

- receives or stores entity-bearing API data;
- renders an entity or related-entity reference;
- reads protected fields;
- exposes entity actions;
- constructs mutation payloads;
- applies mutation results;
- hydrates partial entities or relations.

Each inventory location must identify:

- file and component/function;
- entity or projection;
- service method/endpoint family;
- named adapter or projection;
- partial/complete source and required fields;
- Viewer context and relationship inputs;
- entity-owned representation;
- action builder and command, when applicable;
- mutation and hydration strategy;
- status: `MIGRATED`, `EXCEPTION`, `BLOCKED_BY_BACKEND`, or `INCOMPLETE`.

Do not automatically mark locations migrated because their entity folder exists.

### Definition of done

- Every relevant production file has been inspected.
- Every entity-bearing service method has at least one corresponding inventory location.
- Every mandatory location named by the main plan is included.
- No broad entity-level row substitutes for individual production locations.
- No location has an unknown adapter, representation, access boundary, mutation strategy, or status.

---

## Task 2 — Replace the remaining page-owned entity representations

Move all remaining reusable entity meaning and field markup into the owning entity domains.

### Known required targets

- Admin Order Assistance list, status, identity, message, and detail representations.
- Admin permission-management User references, including protected email/name rendering.
- Review moderation cards and selected Review details.
- Any remaining raw entity rows, cards, summaries, status blocks, or related-entity references found
  by Task 1.

### Direction

For each location:

1. Adapt the source through its named adapter.
2. Build access from the application Viewer and explicit context.
3. Render an entity-owned `Summary`, `Row`, `Card`, `Details`, or justified workflow component.
4. Use the related entity's own representation for embedded User, Vendor, Product, Order, or other
   references.
5. Remove the replaced page-owned markup.

Page-specific layout and workflow controls may remain in the page when they do not define entity
fields, labels, status meaning, visibility, or actions.

### Definition of done

- Repeated entity field markup exists only in the owning entity domain.
- Pages no longer reconstruct entity cards, rows, details, or protected identity labels.
- Related entities use their own canonical reference/summary components.
- Every representation discovered by Task 1 is migrated or has a valid plan-approved exception.

---

## Task 3 — Remove remaining local entity interpretation

Centralize any remaining entity-specific status, formatting, protected-field, ownership, and
transition interpretation.

### Direction

Move remaining logic into:

- entity selectors for labels, styles, formatting, derived state, counts, and availability;
- field-state/access readers for protected values;
- access builders for ownership and visibility;
- action builders for legal operations and transitions;
- projection selectors for analytics/search-specific meaning.

Known examples include:

- locally formatted Report status labels;
- direct status rendering in Admin Assistance;
- direct protected User fields in Admin permission management;
- Review moderation score/style and field interpretation;
- any additional direct enum, status, date, money, ID, stock, permission, or protected-field logic
  discovered by Task 1.

Filtering by a canonical value is allowed. Defining what the value means is not.

### Definition of done

- Pages contain no entity-specific status label/style maps.
- Pages contain no entity-specific date, money, rating, percentage, or shortened-ID formatting.
- Protected fields are read only through access-aware, field-state-aware entity functions or
  presentations.
- Pages do not infer ownership, permissions, or legal transitions.
- A change to entity meaning requires changing the entity/projection domain, not production pages.

---

## Task 4 — Close remaining write and hydration boundaries

Ensure every production mutation and hydration path preserves the canonical architecture.

### Direction

For every action-bearing or form location identified by Task 1:

- expose operations only through the owning action builder;
- use a centralized command mapper for payload semantics;
- adapt authoritative mutation responses through a named adapter;
- use canonical patch/merge helpers when the backend returns no entity;
- preserve identity, loaded fields, partial state, relations, and issues;
- keep hydration in controllers/stores/shared hydration utilities;
- authorize hydration before requesting protected data;
- cache or batch repeated related-entity hydration;
- never fetch from semantic presentation components.

If a backend permission or mutation contract is unsafe or missing, withhold the action and record
`BLOCKED_BY_BACKEND`.

### Definition of done

- No page independently determines whether an entity operation is legal.
- No production form independently defines backend payload rules.
- No mutation turns a canonical entity into an ordinary object.
- Every mutation result is adapted, refetched, or canonically patched.
- Every hydration path has one controller/store owner and a clear cache/batch strategy.
- No semantic presentation imports a service or performs fetching.

---

## Task 5 — Remove superseded architecture and strengthen closure checks

Delete the old implementation after each replacement and make architectural regressions detectable.

### Direction

Remove:

- replaced page/modal entity markup;
- local entity formatters and status helpers;
- local permission, ownership, and transition logic;
- duplicate payload builders;
- raw canonical-entity object patches;
- obsolete adapter aliases and DTO fallbacks;
- dead hydration code;
- duplicate workflow implementations;
- empty or obsolete modal-owned entity sections.

Update architectural checks so they fail for the concrete patterns found during Tasks 1–4. Do not
use tests as a substitute for reading the frontend; checks are only guardrails.

### Definition of done

- One authoritative implementation remains for each entity rule and reusable representation.
- Repository searches have no unexplained architectural violations.
- The inventory contains no `INCOMPLETE` or unknown locations.
- Architectural checks detect the violations found during this completion pass.
- No backend file was modified.

---

## Task 6 — Fresh functional completion audit

After Tasks 1–5 are finished, do not rely on the inventory statuses or previous completion claims.
Re-evaluate Phase 2 from the beginning.

### Required re-read

Read again, in this order:

1. `ENTITY_UI_DOMAIN_UNIFICATION_PLAN.md`, including Phase 1 contracts and Phase 2 requirements.
2. `phase_2_remaining_tasks.md`.
3. This completion plan.
4. The current frontend production files, not only changed files.

### Required audit questions

For every production location, confirm:

1. Is entity-bearing data adapted at the response boundary?
2. Is canonical state retained through rendering and mutation?
3. Is the representation entity-owned?
4. Are related entities represented by their own domains?
5. Are protected and partial fields handled through access and field state?
6. Does access use the single application Viewer and explicit context?
7. Are actions emitted by the owning action builder?
8. Are payload semantics owned by a command?
9. Are mutation results adapted or canonically patched?
10. Is hydration authorized, controller-owned, and cached/batched where repeated?
11. Was the old implementation removed?

Actively search for counterexamples. Do not mark the plan complete merely because tests and the build
pass.

### Definition of done

- The final audit discovers no unresolved functional architectural violation.
- Every production inventory location is `MIGRATED`, a plan-approved `EXCEPTION`, or safely
  `BLOCKED_BY_BACKEND`.
- No location remains `INCOMPLETE` or unknown.
- All architectural checks and the production build pass after the final review.
- The final report explicitly distinguishes:
  - `PHASE_2_FUNCTIONALLY_COMPLETE`, when all conditions above are true; or
  - `PHASE_2_FUNCTIONALLY_INCOMPLETE`, with exact remaining files and tasks.

The agent must not claim completion when any counterexample remains.

