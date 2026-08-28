# Entity Modal Unification Plan

This document tracks frontend entity displays that should move toward canonical modal/page components with section-level RBAC.

## Principles

- Each entity gets one canonical modal composed from independently gated sections.
- Each section accepts partial entity data and renders only when the viewer can read that data.
- The backend remains the source of identity and permissions; frontend checks only decide what already-fetched fields are displayed.
- A single account can be a customer, vendor, admin, and super admin at the same time, so modal permissions are additive.

## Entities

| Entity | Modal Target | First Locations |
| --- | --- | --- |
| User | `frontend/src/components/modals/UserModal.jsx` | Admin users, vendor order customer details, public profile, self profile read summary |
| Vendor | `frontend/src/components/modals/VendorModal.jsx` | Vendor profile, admin vendors, vendor catalog, global search |
| Order | `frontend/src/components/modals/OrderModal.jsx` | Orders, order details, vendor orders, admin orders |
| Product | `frontend/src/components/modals/ProductModal.jsx` | Catalog, product details, vendor dashboard, cart, checkout, order items |
| Gift Flow | `frontend/src/components/modals/GiftFlowModal.jsx` | Gift flow catalog, vendor flow studio, vendor profile, admin flows |
| Vendor Application | `frontend/src/components/modals/VendorApplicationModal.jsx` | Admin vendor applications, user application history, pending vendor review |
| Review | `frontend/src/components/modals/ReviewModal.jsx` | Vendor reviews, product reviews, moderator reviews, my reviews |
| Admin / Super Admin | Extension of `UserModal` | Admin permissions, admin request review, user system sections |

## User Modal Status

`UserModal` is the first implemented entity modal. It supports public identity, protected contact details, protected addresses, admin status, admin request history, admin actions, and super-admin system data.

Current replacements:

- Admin users detail view uses `UserModal`.
- Admin top paying users can open `UserModal` with the partial customer shape.
- Vendor orders customer summaries can open `UserModal` with the partial order customer shape.
- Super-admin/admin permission editor can open the selected admin user through `UserModal`.

Known follow-up:

- Convert public profile page into a page-mode `UserModal`.
- Add user modal entry points to order/customer sections when `OrderModal` is implemented.
- Add backend-backed hydration for partial customer records when a viewer can access full user details.

## Vendor Modal Status

`VendorModal` is implemented as the second canonical entity modal. It supports public store identity, contact/location details, social links, product previews, gift flow previews, review slots, admin status, financial analytics, admin actions, and super-admin system IDs.

Current replacements:

- Admin vendor detail view uses `VendorModal`.
- Admin top selling vendors can open `VendorModal` with the partial analytics vendor shape.
- Public vendor profile uses page-mode `VendorModal` with products, flows, reporting, and reviews preserved.
- Vendor catalog cards use `VendorSummaryCard` and can open `VendorModal` as a preview.

Known follow-up:

- Add richer backend hydration for catalog/admin partial vendor records.
- Move admin vendor applications to a dedicated `VendorApplicationModal`.
- Reuse `VendorModal` inside future `ProductModal` and `OrderModal` vendor sections.

## Order Modal Status

`OrderModal` is implemented as the third canonical entity modal. It supports order header, item metadata/personalization, shipping, payment, customer drill-in, vendor status controls, customer cancel action slot, commission data, assistance threads, and admin/system IDs.

Current replacements:

- Customer order detail page uses page-mode `OrderModal`.
- Vendor order detail overlay uses modal-mode `OrderModal`.
- Vendor order customer sections can open `UserModal` from inside `OrderModal`.
- Admin dashboard order detail view uses `OrderModal`.

Known follow-up:

- Add real customer cancellation once the backend/API flow is available.
- Add order preview entry points from the order history list if inline details become preferable to navigation.

## Product Modal Status

`ProductModal` is implemented as the fourth canonical entity modal. It supports public product identity, pricing/discounts, taxonomy, gift options, delivery/recipient/composition requirements, protected inventory/vendor/SEO data, admin actions, and admin/system IDs. It accepts full product records, admin analytics rows, product search rows, and order item snippets, then hydrates partial records by product ID when a full product page endpoint is available.

Current replacements:

- Admin product detail view uses `ProductModal`.
- Admin top selling products can open `ProductModal` with the partial analytics product shape.
- `OrderModal` item rows can open `ProductModal` from product image/name while retaining a direct product-page link.
- Vendor inventory rows can open `ProductModal`.
- Vendor analytics top products can open `ProductModal`.

Known follow-up:

- Convert product details page into page-mode `ProductModal` while preserving cart, favorite, report, and review slots.
- Add product modal preview entry points to catalog, cart, checkout, favorites, gift flow step selections, vendor dashboard, and vendor analytics.
- Consider a `ProductSummaryCard` once catalog/cart/vendor product cards are unified enough to share one compact representation.

## Gift Flow Modal Status

`GiftFlowModal` is implemented as the fifth canonical entity modal. It supports public flow identity, cover image, description, parsed step structure, required/single/multiple selection rules, product option constraints, product drill-ins through `ProductModal`, protected vendor data, raw configuration review, action slots, and system IDs/timestamps. It accepts partial flow rows and hydrates the full flow by ID when needed. It also hydrates referenced products from the flow configuration so the modal can render product names/images instead of only product IDs.

Current replacements:

- Gift flow catalog cards can open `GiftFlowModal` without leaving the catalog.
- Favorite gift flows can open `GiftFlowModal`.
- Vendor profile/vendor modal flow previews can open `GiftFlowModal`.
- Product detail "also available in gift flows" references open `GiftFlowModal` while retaining a public flow link inside the modal.
- Vendor gift flow studio can preview the saved selected flow through `GiftFlowModal`.

Known follow-up:

- Convert the gift flow step page header/sidebar into page-mode `GiftFlowModal` slots while preserving the active builder workflow.
- Add `GiftFlowModal` entry points to home page flow cards and global search flow results.
- Wire admin gift-flow management if/when an admin flows panel exists.

## Review Modal Status

`ReviewModal` is implemented as the sixth canonical entity modal. It supports public review content, rating, author anonymity, status, review target IDs/links, order linkage, moderation score, moderator notes, approve/reject action slots, and system IDs/timestamps.

Current replacements:

- Public `ReviewList` cards can open `ReviewModal`.
- `MyReviews` cards can open `ReviewModal` with self/status context.
- `ModeratorReviews` pending review detail panel can open `ReviewModal` with moderation actions.

Known follow-up:

- Convert the moderator review detail panel itself to page-mode `ReviewModal` after vendor feedback gets its own modal.
- Add user drill-in through `UserModal` for non-anonymous reviews when permissions allow it.
- Add product/vendor/gift-flow target drill-ins through the relevant entity modal instead of only target links.
