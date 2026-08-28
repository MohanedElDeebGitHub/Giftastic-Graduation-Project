# Entity Modal Unification Guide

## What We Are Doing

Giftastic has many frontend places that display the same entity differently. A user, vendor, order, product, or gift flow might have one shape in an admin table, another in a public profile, and another inside an order view. That makes UI changes slow and creates inconsistent data visibility.

We are solving this by creating one canonical modal/page component per major entity. Each modal is made of small sections, and each section decides whether it can render based on the current viewer's permissions and the data available.

The backend remains the source of truth for identity, relationships, and permissions. The frontend modal only controls how already-returned data is represented.

## Target Pattern

Each entity should have:

- A root modal component, for example `UserModal`, `VendorModal`, `OrderModal`.
- Small section components, for example `UserIdentitySection`, `UserContactSection`, `VendorAnalyticsSection`.
- Permission/access helpers close to the modal, for example `userModalUtils.js`.
- A summary component when lists need a compact representation, for example `UserSummaryButton` or `VendorSummaryCard`.
- Page-mode support when a full page should use the same entity representation instead of duplicating it.

Example shape:

```jsx
<UserModal
  user={user}
  viewer={currentUser}
  permissions={currentPermissions}
  onClose={closeModal}
  onAction={handleAdminAction}
/>
```

## RBAC Rules

A modal must be modular because one account can be a normal user, vendor, admin, and super admin at the same time.

Use additive checks:

- Public sections render for everyone.
- Self sections render when `viewer.id === entity.id`.
- Admin sections render only for the relevant permission.
- Super admin sections render only for `SUPER_ADMIN`.
- Action buttons require both permission and an actual callback wired by the caller.

Do not assume role names are mutually exclusive.

## Pitfalls To Avoid

- Do not create another one-off detail modal inside a page.
- Do not put permission logic only in the parent page; sections should remain independently gateable.
- Do not show sensitive fields just because they exist on the object.
- Do not break partial DTOs. Admin analytics rows, order customer snippets, and search results may only have a few fields.
- Do not fetch new data inside every section by default. Prefer passing data down from the page unless hydration is deliberately designed.
- Do not mix edit forms into read-only modal sections unless the section explicitly supports `mode="edit"`.
- Do not hide backend identity behind frontend guesses. Use backend IDs like `id`, `userId`, `supplierId`, and `customerId`.
- Do not duplicate product/vendor/user cards after a summary component exists.

## How To Add The Next Entity

1. Map every frontend location where the entity appears.
2. Identify the full backend-backed data model and common partial DTOs.
3. Split the modal into public, self/vendor/admin, financial, action, and system sections.
4. Add access helpers that compute section visibility from `viewer` and `permissions`.
5. Build the modal to tolerate partial data.
6. Replace the richest existing detail view first.
7. Replace list summaries with a shared summary component.
8. Convert full pages to page-mode modal usage when practical.
9. Run `npm run build`.
10. Update `entity_modal_unification_plan.md`.

## Current Progress

- `UserModal` is implemented and used in admin users, super-admin/admin permission review, vendor order customer views, and public user profile.
- `VendorModal` is implemented and used in admin vendors, public vendor profile, and vendor catalog.
- `OrderModal` is implemented and used in customer order details, vendor order details, and admin order details.
- `ProductModal` is implemented with partial-record hydration and used in admin products, vendor inventory/analytics, plus order item drill-ins.
- `GiftFlowModal` is implemented with configuration/product-option hydration and used in catalog, favorites, vendor previews, product detail flow references, and vendor flow studio preview.
- `ReviewModal` is implemented and used from public review lists, customer review history, and moderation review details.

Recommended next major entity: `VendorApplicationModal`, because vendor onboarding still has custom admin/user views.
