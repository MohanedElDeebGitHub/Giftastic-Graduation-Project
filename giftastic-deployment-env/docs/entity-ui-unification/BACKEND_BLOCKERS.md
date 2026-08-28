# Backend Contract Register

Re-audited `2026-06-20` against branch `continue/entity-ui-domain-unification-farid`, baseline HEAD
`150792324d0f1dceb0100b13b85c824fe282784a`. No backend file was modified.

No backend file is modified by Phase 1. These audited inconsistencies are carried from the
authoritative plan and must be resolved server-side for system certification.

| # | Recommendation | Frontend behavior and risk |
| ---: | --- | --- |
| 1 | Product rejection should check `REJECT_PRODUCTS`, not `ACTIVATE_PRODUCTS`. | Withhold/label the action according to the exact current contract; mismatch risks incorrect authorization. |
| 2 | Add a distinct Vendor Feedback mutation permission. | Approval/rejection currently relies on read permission; avoid implying a stronger security boundary. |
| 3 | Move Order mutation authorization from `@PostAuthorize` to pre-mutation checks. | UI gating cannot prevent an unauthorized server-side mutation. |
| 4 | Prove ownership in Notification `markAsRead(id)`. | The UI scopes actions to the owner, but server enforcement remains mandatory. |
| 5 | Return least-data public Product projections. | Public domain responses may deliver Vendor/admin fields that the UI must mark forbidden. |
| 6 | Confirm whether public reads of all Vendor delivery pricing are intentional. | UI does not treat public delivery-pricing access as ownership authorization. |
| 7 | Separate broad roles from fine-grained permissions in auth responses. | The canonical session adapter normalizes the mixed authority list once. |
| 8 | Supply a truthful account creation date for `memberSince`. | The current `LocalDate.now()` value must not be shown as membership history. |
| 9 | Retire or designate canonical Vendor Application, Order Assistance and Favorite endpoint families. | Each consumed family needs an explicit named adapter; ambiguity increases drift risk. |
| 10 | Align Product create/update DTO/controller mapping with Product Details delivery fields. | Unsupported fields remain unavailable in command payloads; silently claiming persistence would lose data. |
| 11 | Keep `isCommunityHelper` as a projection owned by the User/Admin adapter. | Inferring it locally from a primary role would break additive facets. |
| 12 | Add an authorized Category update endpoint or explicitly declare Categories immutable after creation. | The previous Admin UI called a non-existent `/admin/categories/{id}` PUT route. Phase 2 withholds the broken edit action while preserving create/delete. |
| 13 | Include current stock availability in `CartResponse`, or provide a batch Product availability endpoint. | Checkout stock preflight otherwise needs one Product lookup per unique cart item. The frontend now caches those lookups, but a backend batch/source-owned availability contract is safer and more efficient. |
