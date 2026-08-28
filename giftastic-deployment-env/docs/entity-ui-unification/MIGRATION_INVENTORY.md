# Phase 2 Migration Inventory

## Tasks 1-6 functional inventory refresh — 2026-06-21

The executable production-consumer inventory now lives in
`frontend/src/ui/entities/productionMigrationInventory.js`. It contains one closed row for each of
the 23 entity domains plus explicit per-location records. Each location declares its concrete
component or boundary, parent consumer, service methods, named source adapter, endpoint family,
source completeness, canonical fields, viewer/context inputs, presentation owner, hydration owner,
actions, command boundary, mutation strategy, status, and evidence. Adapters are no longer applied
to consumers through a generated cross-product.

The registry is validated by `phase2Migration.test.js`:

- every registered consumer file must exist;
- every named adapter must be registered;
- all 23 domains must be present exactly once;
- each domain must declare production representation ownership;
- Product, Order, Gift Flow, and Vendor semantic sections must live under their entity domains;
- the high-use Admin/Vendor management representations must import entity-owned rows/cards.

Final functional audit totals: 23 domains, 127 explicit locations, and 145 production service
methods. Statuses are 126 `MIGRATED` and one `BLOCKED_BY_BACKEND` Vendor Feedback moderation
location. There are no `INCOMPLETE` or unknown locations.

Tasks 1-5 use the following exception classifications from the normative plan:

- Product create/edit, Vendor Application, Review, Report, Feedback, payment proof, and similar
  submission forms are command-form/submission-modal exceptions;
- Cart and Order Item Product data are historical snapshot representations;
- Product Details, Gift Flow execution/editor, Cart, Checkout, moderation, and financial screens may
  remain specialized workflow compositions, but their entity identity, access, actions, formatting,
  and reusable sections are owned by entity/projection/command domains;
- analytics metrics remain projection-owned while linked entities use canonical references.

This refresh supersedes older `NOT_STARTED` statements below for the response/session,
representation, and modal-ownership scope of Tasks 1-5. Remaining command, action, formatting,
protected-field, mutation, hydration, cleanup, and QA closure work is tracked by
`phase_2_remaining_tasks.md`.

Inventory baseline refreshed on branch `phase2-start-omar`, base HEAD
`5335b6fbb023a0ae48340dbb9c17fad86e402acf`, on `2026-06-20`.

Phase 2 has reached the 97% incremental checkpoint. The domain table remains the overall scope
summary; concrete tables cover each migrated dependency slice.

| Domain | Existing production consumers (representative, source-audited) | Phase 2 status |
| --- | --- | --- |
| User | `UserProfile`, `PublicUserProfile`, `UserDashboard`, `UserModal`, `UserSummaryButton` | IN_PROGRESS (session, public, Admin, Vendor-order, and self-profile sources migrated) |
| Vendor | `VendorCatalog`, `VendorProfile`, `VendorDashboard`, `VendorSettings`, `VendorModal`, `VendorSummaryCard` | IN_PROGRESS (catalog/profile/settings/Admin detail slice migrated) |
| Product | `ProductCatalog`, `ProductDetails`, `ProductSearch`, `ProductRecommendations`, `ProductModal` and product modal sections | IN_PROGRESS (public/details/modal/vendor/admin/analytics detail slice migrated) |
| Order | `Orders`, `OrderDetails`, `VendorOrders`, `OrderModal` and order modal sections | IN_PROGRESS (customer/vendor/admin details and named list sources migrated) |
| Gift Flow | `GiftFlowCatalog`, `GiftFlowStep`, `VendorGiftFlows`, `GiftFlowModal` and sections | IN_PROGRESS (public catalog/execution, named projections, preview controller/modal migrated) |
| Cart | `Cart`, `Checkout`, cart store/service consumers | IN_PROGRESS (named store, access, checkout command migrated) |
| Review | `ReviewList`, `MyReviews`, `ModeratorReviews`, `ReviewModal` and sections | IN_PROGRESS (sources, summaries, modal, commands migrated) |
| Category | `ProductCatalog`, `CategoryModal` and category sections | IN_PROGRESS (Admin management and modal migrated) |
| Vendor Application | `BecomeVendor`, `MyVendorApplications`, `AdminVendorApplications`, modal and sections | IN_PROGRESS (sources, commands, actions, modal migrated) |
| Commission | `VendorCommissions`, `AdminFinancial`, `CommissionModal` and overview | IN_PROGRESS (named lists/detail/actions migrated) |
| Commission Payment Request | `VendorCommissions`, `AdminFinancial` | IN_PROGRESS (named lists/detail/actions migrated) |
| Commission Rule | `AdminFinancial` | IN_PROGRESS (named lists/detail/actions migrated) |
| Report | `AdminReports`, report service consumers | IN_PROGRESS (moderation and submission boundaries migrated) |
| Admin Request | `AdminDashboard`, `UserProfile`, `AdminRequestModal` and sections | IN_PROGRESS (sources/actions/modal migrated) |
| Order Assistance | `OrderDetails`, `OrderAssistanceSection`, vendor/admin order surfaces | IN_PROGRESS (named sources/access/actions migrated) |
| Notification | `Notifications`, `NotificationBell`, `NotificationModal` and sections | IN_PROGRESS (owner list/actions/modal migrated) |
| Vendor Feedback | `ModeratorReviews`, review service consumers | IN_PROGRESS (named source/semantic summary; unsafe mutations withheld) |
| Delivery Zone | `ZoneSelector`, `Checkout`, delivery service consumers | IN_PROGRESS (selector/checkout sources migrated) |
| Vendor Delivery Pricing | `VendorDeliveryPricing`, `VendorSettings` | IN_PROGRESS (owner response/editor boundary migrated) |
| Reminder | reminder service/profile consumers | IN_PROGRESS (dashboard named source/ownership migrated) |
| Vendor Activity | `VendorActivityDashboard`, vendor activity service consumers | IN_PROGRESS (named source/ownership migrated) |
| User Review Restriction | `MyReviews`, User moderation facets | IN_PROGRESS (self response/summary migrated; browser pending) |
| Favorite | `Favorites`, product/flow favorite controls | IN_PROGRESS (reads/access/actions use canonical relationship) |

Cross-domain consumers requiring projection migration include `GlobalSearch`, `HomePage`,
`AdminDashboard`, `VendorAnalytics`, and authentication/session boundaries.

## Concrete User/session inventory — first 5%

`Partial` means the adapter may expose only the fields declared by that named source. “Page” owns
fetching/hydration unless another owner is shown.

| Representation / route | Service source | Named adapter; completeness; required fields | Viewer / context | Semantic view; owner; actions | Status / evidence |
| --- | --- | --- | --- | --- | --- |
| Auth session; app boundary (`authService`, auth store, `ProtectedRoute`, `Navbar`) | `/auth/login`, `/auth/register`, local persisted session | `adaptUserAuthSession`; Partial; id, email, fullName, banned state, vendor/admin facets | canonical application Viewer | no entity view; auth service/store; route capabilities | MIGRATED; tests + build |
| `PublicUserProfile`; `/users/:userId` | `userService.getPublicProfile` | `adaptUserPublicProfile`; Partial; id, fullName, community facet | guest/authenticated; `PUBLIC` | `UserDetails` through thin `UserModal`; Page; none | MIGRATED; boundary test + build |
| Admin all-users summary/detail; Admin dashboard | `adminService.getAllUsers` | `adaptUserAdminManagementRecord`; Partial; management fields and admin facets | exact admin permissions; `ADMIN_READ` | `UserSummary`/`UserDetails`; Panel; canonical ban/unban/promote/demote | MIGRATED shared views/actions; tests + build |
| Admin all-users contact/status table; Admin dashboard | `adminService.getAllUsers` | same; Partial; phone, birthday, addresses, banned state | exact admin permissions; `ADMIN_READ` | page-owned legacy cells; Panel; canonical ban/unban | IN_PROGRESS; shared pieces migrated, table markup remains |
| Top-paying User reference; Admin dashboard | `adminService.getPlatformAnalytics` | `adaptUserAnalyticsCustomerReference`; Partial; id, fullName | `VIEW_FINANCIAL_ANALYTICS`; `ADMIN_FINANCIAL` | `UserSummary`/partial `UserDetails`; Panel; none | MIGRATED; boundary test + build |
| Admin permission target summary/detail; Admin dashboard | composed `getAllUsers` + `getAllAdmins` | `adaptUserAdminManagementRecord`; Partial; identity and admin facets | permission-management viewer; `ADMIN_MANAGEMENT` | `UserSummary`/`UserDetails`; Panel; grants remain page workflow | MIGRATED representation; tests + build |
| Vendor order customer summary/detail; `/vendor/orders` | order vendor-list customer snapshot | `adaptUserOrderCustomerSnapshot`; Partial; id, fullName, email | participating Vendor; `ORDER_VENDOR` | `UserSummary`/`UserDetails`; Page; none | MIGRATED; boundary test + build |
| Order customer section inside `OrderModal` | Order customer snapshot | `adaptUserOrderCustomerSnapshot`; Partial; id, fullName, email | participating Vendor/admin; order-derived context | `UserSummary`; modal section currently adapts/builds access | IN_PROGRESS; move ownership to controller |
| Self profile/editor; profile routes | User/profile services | source to confirm from concrete consumer | self; `SELF` | semantic detail/editor target; Page; profile commands | NOT_STARTED |
| Checkout User defaults/identity | checkout + User services | source to confirm from concrete consumer | self; checkout context | command/default mapping; Page; checkout action | NOT_STARTED |

## Concrete Vendor inventory — 20% checkpoint

| Representation / route | Service source | Named adapter; completeness; required fields | Viewer / context | Semantic view; owner; actions | Status / evidence |
| --- | --- | --- | --- | --- | --- |
| Vendor cards; `/vendors` | `vendorService.getAllVendors` | `adaptVendorPublicListRecord`; Partial; storefront fields | Guest/User; `PUBLIC` | `VendorSummary`; Page; none | MIGRATED; controller + boundary test + browser |
| Vendor profile/details; `/vendors/:supplierId` | public Vendor list + Vendor Products/Flows | named Vendor/Product/Gift Flow adapters; Partial composite | Guest/User; `PUBLIC` | `VendorDetails`; Page; report workflow external | MIGRATED; canonical relations + build |
| Owner settings preview; `/vendor/settings` | `getMyVendorProfile`, `updateVendorProfile` | `adaptVendorMe`; Complete when backend supplies fields | Vendor owner; `OWNER_MANAGEMENT` | `VendorDetails`; Page; edit command stays form | MIGRATED representation; tests + build |
| Admin Vendor details | `adminService.getAllVendors`, pending Vendors | `adaptVendorDomain`; Complete | exact Vendor permissions; `ADMIN_MANAGEMENT` | `VendorDetails`; Admin panel; canonical activate/deactivate actions | MIGRATED detail; legacy Admin row markup remains |
| `VendorModal` / `VendorSummaryCard` | controller-owned | already canonical | supplied access | thin containers only | MIGRATED; static boundary test |
| Vendor dashboard/analytics embedded representations | Vendor/analytics services | named sources exist | owner/admin | canonical references required | NOT_STARTED |

## Concrete Product inventory — 20% checkpoint

| Representation / route | Service source | Named adapter; completeness; required fields | Viewer / context | Semantic view; owner; actions | Status / evidence |
| --- | --- | --- | --- | --- | --- |
| Product Catalog cards; `/products` | `productService.getProducts` | `adaptProductDomain`; Complete when supplied | Guest/User; `PUBLIC` | `ProductSummary`; Page; Favorite decoration | MIGRATED; protected stock omitted |
| Product Search results | `productSearchService.searchWithFilters` | `adaptProductSearchResult`; Partial | Guest/User; `SEARCH` | `ProductSummary`; component controller; none | MIGRATED; tests + build |
| Product Recommendations | `recommendationService.*` | `adaptProductRecommendationReference`; Partial | Guest/User; `SUMMARY` | `ProductSummary`; component controller; none | MIGRATED; duplicate card removed |
| Product details page | `productService.getProductById` | `adaptProductDomain`; Complete | Guest/User; `PUBLIC` | public Product workflow plus Review/Gift Flow references | IN_PROGRESS; named source/access/formatting migrated, bespoke page layout remains |
| Product modal | controller-owned | already canonical | public/owner/admin supplied access | thin details container; controller-owned actions/hydration | MIGRATED; boundary tests |
| Vendor inventory Product details | `productService.getVendorProducts` | `adaptProductDomain`; Complete | Vendor owner; `OWNER_MANAGEMENT` | `ProductModal`; Vendor page | MIGRATED detail; legacy table row remains |
| Vendor analytics Product detail | analytics reference + `getProductById` | analytics reference then domain hydration | Vendor owner | `ProductModal`; analytics controller | MIGRATED detail; projection table formatting remains |
| Admin Product detail | Admin lists/analytics + `getProductById` | domain or analytics reference then domain hydration | exact Product permission; `ADMIN_MODERATION` | `ProductModal`; Admin controller; canonical actions | MIGRATED detail; legacy Admin rows/actions remain |

## Concrete Order inventory — 30% checkpoint

| Representation / route | Service source | Named adapter; completeness; required fields | Viewer / context | Semantic view; owner; actions | Status / evidence |
| --- | --- | --- | --- | --- | --- |
| Customer Order history; `/orders` | `orderService.getCustomerOrders` | `adaptOrderCustomerListRecord`; Partial | customer; `CUSTOMER` | `OrderHistoryCard`; Page controller | MIGRATED representation; live fixture pending |
| Customer Order details; `/orders/:orderId` | `orderService.getOrderById` | `adaptOrderDomain`; Complete | customer; `CUSTOMER` | `OrderModal` inline details; Page controller | MIGRATED representation; live fixture pending |
| Vendor Order list/details; `/vendor/orders` | `orderService.getVendorOrders` | `adaptOrderVendorListRecord`; Partial | participating Vendor; `VENDOR` | legacy row + canonical details; Page controller | IN_PROGRESS; details/container/access migrated |
| Admin Order details | Admin Order list | `adaptOrderAdminListRecord` target; current list adaptation predates this slice | exact Order permission; `ADMIN` | canonical `OrderModal`; Admin controller | IN_PROGRESS; details migrated, list source cleanup remains |
| Order customer reference | Order customer snapshot | `adaptUserOrderCustomerSnapshot`; Partial | participating Vendor/Admin | `UserSummary`; controller-owned access | MIGRATED; modal section no longer adapts |
| Order Item Product reference | historical Order Item snapshot | `adaptProductOrderItemSnapshot` contract | customer/Vendor/Admin | historical snapshot with current Product page link | MIGRATED boundary; no current Product substitution |
| `OrderModal` | controller-owned | already canonical | supplied Order/access/related access | thin container only | MIGRATED; boundary tests |

## Concrete Gift Flow inventory — 35% checkpoint

| Representation / route | Service source | Named adapter; completeness; required fields | Viewer / context | Semantic view; owner; actions | Status / evidence |
| --- | --- | --- | --- | --- | --- |
| Public Flow catalog; `/gift-flow` | `giftFlowService.getAllFlows` | `adaptGiftFlowResponse`; Partial/complete by payload | Guest/User; `PUBLIC` | `GiftFlowSummary`; Page; Favorite decoration | MIGRATED; tests + responsive browser |
| Executable Flow; `/gift-flow/:flowId` | `giftFlowService.getFlowById` + Product details | `adaptGiftFlowResponse`, `adaptProductDomain` | Guest/User; public execution | specialized Flow workflow; Page controller | IN_PROGRESS; named sources migrated, command extraction pending |
| Gift Flow modal/details | controller-owned Flow + Product sources | named Flow/Product adapters | public/owner/admin | thin `GiftFlowModal`; shared controller | MIGRATED boundary; live populated state pending |
| Vendor Flow studio previews | Vendor Flow/Product services | `adaptGiftFlowResponse`, `adaptProductDomain` | Vendor owner; `OWNER` | controller + canonical details; editor remains command form | IN_PROGRESS |
| Home Flow references | public Flow list | `adaptGiftFlowResponse` | Guest/User; summary | `GiftFlowSummary` | MIGRATED representation |
| Global Search Flow results | unified search | `adaptGiftFlowUnifiedSearchResult`; Partial | Guest/User; search | compact search reference | MIGRATED source; specialized dropdown retained |
| Favorites Flow references | Favorite relationship + Flow detail | `adaptFavoriteLegacyRecord`, `adaptGiftFlowResponse` | owner | `GiftFlowSummary` + canonical remove decoration | MIGRATED representation; live fixture pending |

## Concrete Cart through financial inventory — 57% checkpoint

| Entity / representation | Production location and source | Named adapter / command | Viewer / semantic owner | Status / evidence |
| --- | --- | --- | --- | --- |
| Cart store, authenticated and guest | `useCartStore`; `cartService.getCart`, local guest cart | `adaptCartResponse`, `adaptGuestCart`; cached `adaptProductDomain` hydration | owner or local Guest; store owns hydration | MIGRATED boundary; tests + build |
| Cart workflow | `/cart` | canonical Cart model/selectors/access | application Viewer; page workflow | IN_PROGRESS; canonical data/access, page-owned item controls remain |
| Checkout | `/checkout`; User/Delivery/Cart sources | `adaptUserMe`, `adaptDeliveryZoneResponse`, checkout command mapping | User or Guest; page owns command | MIGRATED payload/default boundary; live payment/delivery fixtures pending |
| Public Review list/detail | `ReviewList`; `getReviewsByEntity` | `adaptReviewPublicResponse` | Guest/User; `ReviewSummary`/thin detail dialog | MIGRATED; tests + build |
| Self Review list/detail | `/my-reviews`; `getMyReviews` | `adaptReviewSelfResponse` | owner; `ReviewSummary`/thin detail dialog | MIGRATED; tests + build |
| Review moderation | `/moderator/reviews`; pending reviews | `adaptReviewModerationResponse` | exact moderator Viewer; canonical access/actions/detail | IN_PROGRESS; canonical entity boundary, page workflow markup remains |
| Review/Vendor Feedback submissions | `ReviewForm`, `VendorFeedbackModal` | `reviewSubmission`, `feedbackSubmission` commands | submitting User | MIGRATED command boundary |
| Category management/detail | Admin dashboard; `adminService.getCategories/create/delete` | `adaptCategoryListRecord` | `MANAGE_CATEGORIES`; canonical summary/detail/actions | MIGRATED current backend operations; update blocked by backend recommendation 12 |
| Vendor Application submission | `/become-vendor` | `vendorApplication` command | applicant | MIGRATED command boundary |
| Vendor Application self/Admin lists and detail | profile/application routes; `vendorApplicationService.*` | `adaptVendorApplicationResponse` | owner or exact reviewer; canonical summary/actions/thin detail | MIGRATED boundary; live matrix pending |
| Commission Vendor/Admin lists/detail | `/vendor/commissions`, Admin Financial/Dashboard | `adaptCommissionDto` | owner or financial Admin; canonical summary/access/actions/detail | MIGRATED boundary; legacy tables remain |
| Commission Payment Request lists/detail | same financial routes | `adaptCommissionPaymentRequestDto`, `commissionProof` command | owner/reviewer; canonical access/actions/detail | MIGRATED boundary; live proof workflow pending |
| Commission Rule lists/detail | Admin Financial/Dashboard | `adaptCommissionRuleDto`, existing rule command | exact management Viewer; canonical access/actions/detail | IN_PROGRESS; list/detail migrated, create forms still page-owned |

## Concrete moderation and delivery inventory — 77% checkpoint

| Entity / representation | Production location and source | Named adapter / command | Viewer / semantic owner | Status / evidence |
| --- | --- | --- | --- | --- |
| Report submission | `ReportButton`; `reportService.createReport` | `reportSubmission` command | authenticated reporter; command form | MIGRATED command boundary |
| Report moderation | `/admin/reports`; report service lists | `adaptReportDomain` | hydrated Admin Viewer; semantic summary/thin details/canonical actions | MIGRATED boundary; live matrix pending |
| Admin Request self | `UserProfile`; `getMyRequests` | `adaptAdminRequestDto` | self Viewer; canonical summary/thin details | MIGRATED boundary |
| Admin Request moderation | Admin dashboard; pending requests | `adaptAdminRequestDto` | exact Admin Viewer; canonical access/actions/thin details | IN_PROGRESS; legacy row markup remains |
| Notification page/bell/details | `/notifications`, `NotificationBell`; notification service | `adaptNotificationOwnerRecord` | exact owner; canonical summary/access/actions/thin details | MIGRATED boundary; polling/browser pending |
| Order Assistance Vendor/Admin | Vendor Orders and Admin dashboard assistance services | `adaptOrderAssistanceDto` | participating Vendor/exact Admin; canonical access/actions | IN_PROGRESS; thread section still owns presentation/action assembly |
| Vendor Feedback moderation | Moderator Reviews | `adaptVendorFeedbackResponse` | exact moderator; semantic summary, mutation actions withheld | IN_PROGRESS; backend blocker 2 governs mutations |
| Delivery Zone checkout/selector | `Checkout`, `ZoneSelector`; `getAllZones` | `adaptDeliveryZoneResponse` | checkout context; selector/page controller | MIGRATED response boundary |
| Vendor Delivery Pricing editor | `/vendor/delivery-pricing`; Vendor pricing service | `adaptVendorDeliveryPricingResponse`; local unsaved drafts use canonical factory | Viewer supplier owner; canonical editor/access | MIGRATED fetched boundary; live save/browser pending |

## Concrete final operational inventory — 97% checkpoint

| Entity / representation | Production location and source | Named adapter | Viewer / owner | Status / evidence |
| --- | --- | --- | --- | --- |
| Reminder dashboard cards | `UserDashboard`; `reminderService.getMyReminders` | `adaptReminderDomain` | canonical User owner Viewer; domain summary/access | MIGRATED boundary; creation/browser pending |
| Vendor Activity feed | `/vendor/activity`; activity service | `adaptVendorActivityResponse` | Viewer supplier owner; domain summary/access | MIGRATED boundary; browser pending |
| User Review Restriction self notice | `/my-reviews`; restriction service | `adaptUserReviewRestrictionResponse` | self Viewer; domain summary/access | MIGRATED production representation; browser pending |
| Favorite Product/Flow relationships | Favorites, Product Catalog/Details, Gift Flow Catalog/Step | `adaptFavoriteLegacyRecord` for reads | exact User owner; canonical access/remove decoration | MIGRATED read boundary; live mutation matrix pending |
| Order Assistance modal workflow | Vendor Orders controller + Order modal section | `adaptOrderAssistanceDto` | participating Vendor; controller-built access/actions/messages | MIGRATED ownership boundary; browser pending |
| Admin Order list | Admin dashboard Orders panel | `adaptOrderAdminListRecord` | exact Admin Viewer | MIGRATED source; legacy table markup remains |

## Final cleanup reconciliation

Vendor Analytics now uses `adaptVendorAnalyticsProjection`, canonical Product references, Viewer
supplier ownership, and shared money/rating formatting. Vendor Dashboard, Admin financial/dashboard,
User Profile, Product hero/rating, and Review moderation formatting/command boundaries were likewise
centralized in the final cleanup pass.

The executable registry, rather than the older checkpoint tables below, is the exhaustive
specification-required inventory for the current production tree. Its verification fails when a
consumer file, named source contract, per-location field, or declared entity-bearing service method
is missing.
