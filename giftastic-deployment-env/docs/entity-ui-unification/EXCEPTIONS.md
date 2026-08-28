# Phase 1 Exceptions

No new exception category was requested or self-approved.

No mandatory Phase 1 requirement was intentionally skipped as quality-of-life work.

Phase 2 production migration remains intentionally `NOT_STARTED` under the user's explicit
Phase 1-only boundary; it is not a Phase 1 exception.

## Phase 2 pre-approved concrete instances

| File / symbol | Plan category | Reason | Approval | Removal condition |
| --- | --- | --- | --- | --- |
| `frontend/src/store/useCartStore.js` guest-cart and item metadata serialization | Historical snapshots / command persistence | JSON is transport and local-storage serialization, never a rendered entity view | Specification pre-approval; 2026-06-21 | Remove if storage/API accepts structured values directly |
| `frontend/src/pages/GiftFlowStep.jsx` cart-item metadata | Command workflow | JSON is the backend-supported cart metadata payload, never user-facing object dumping | Specification pre-approval; 2026-06-21 | Replace when a typed backend metadata request exists |
| `frontend/src/pages/VendorDeliveryPricing.jsx` unsaved pricing drafts | Command forms | Local canonical draft rows are editor state; fetched rows use the named adapter | Specification pre-approval; 2026-06-21 | Remove if the backend supplies persisted draft records |
