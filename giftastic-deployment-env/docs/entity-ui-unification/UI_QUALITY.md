# Phase 1 UI Quality Evidence

Tested implementation commit: `a311ccb06c5363779240a1be41dc4458805c0fce`.
Browser route: `http://127.0.0.1:4173/__phase1/entities`.

## Semantic and state coverage

- All 23 domains rendered through the app-hosted harness.
- Favorite renders only as a target decoration/remove workflow, not an independent identity/details
  record.
- Required states verified: complete, partial, invalid, forbidden, loading, empty,
  recoverable-error, action-pending, and ready.
- The backend-blocked User `memberSince` value is not presented as truthful membership history.
- Protected public/forbidden boundaries were rendered alongside owner/system representations.

## Accessibility

- One page-level `h1`; each entity fixture is an `aria-labelledby` region.
- Definition lists preserve field label/value semantics.
- Loading/empty/forbidden use status semantics; invalid/recoverable errors use alert semantics.
- Action controls have accessible text and a minimum measured height of 44px.
- Keyboard verification focused and activated the synthetic `Delete User` action with `Enter`;
  focus displayed a solid outline.
- No duplicate element IDs were found.

## Responsive and overflow matrix

| Viewport | Result |
| --- | --- |
| 320x720 | PASS; no horizontal overflow after long entity-name remediation |
| 375x812 | PASS; no horizontal overflow |
| 768x900 | PASS; no horizontal overflow |
| 1280x900 | PASS; no horizontal overflow |

The browser reported only React Router future-version warnings. No application console errors were
observed.

## Stage 3 semantic regression evidence

At `2026-06-20T21:02:55+03:00`, the server-rendered semantic harness reverified all 23 domains after
explicit section/field access gating and shared date formatting. It additionally asserted that a
participating Order Vendor cannot render another Vendor's item or the customer system identifier,
and that a Notification owner cannot render Notification/User system IDs. Action help text is now
contained inside each button without generated IDs, preventing repeated entity rows from creating
duplicate DOM IDs. The production build completed successfully.

## Phase 2 20% route checks

At `2026-06-21T01:21:29+03:00`, `/vendors` and `/products` were checked in the in-app browser at
320x720, 768x900, and 1280x900. Both routes had semantic `h1` structure and zero horizontal
overflow after responsive Navbar and page-container fixes. Navbar icon links now have accessible
names and 44px controls. `EntityDialog` statically verifies dialog semantics, initial focus,
focus containment/restoration, Escape close, visible focus, and a 44px close target.

The backend API was unavailable; populated cards and modal visual states remain pending rather than
being inferred from unit tests.

## Phase 2 30% UI boundary

`ProductModal` and `OrderModal` now both use the shared accessible `EntityDialog`, preserving the
previously verified focus, Escape, scroll, responsive, and touch-target behavior. Product public
details no longer expose moderation status or exact protected stock counts; availability is
presented semantically while the purchase workflow retains backend stock validation.

Live populated modal visual checks remain pending because the local backend and viewer fixtures were
unavailable.

## Phase 2 35% UI boundary

The public `/gift-flow` route was verified at 320x720, 768x900, and 1280x900 with a semantic `h1`
and no horizontal overflow. Canonical Flow cards provide a 44px named Favorite control and a
separate Details action. `GiftFlowModal` now inherits the shared accessible dialog behavior.

Populated-card and modal visual states remain pending because the local API was unavailable.

## Phase 2 57% UI boundary

Review, Category, Vendor Application, Commission, Payment Request, and Commission Rule details now
use the shared `EntityDialog` focus/Escape/scroll/touch-target contract and centralized semantic
partial/invalid/forbidden rendering. Newly touched form controls have visible focus and modal action
buttons retain 44px minimum targets. No new live-route browser result is claimed because populated
backend fixtures remain unavailable.

## Phase 2 77% UI boundary

Report, Admin Request, and Notification details now inherit the shared `EntityDialog` keyboard,
focus restoration, Escape, responsive scrolling, and minimum-target behavior. Their semantic views
centralize partial, invalid, empty, and forbidden states. Delivery pricing continues to use the
canonical responsive editor. Populated browser evidence remains pending because local API fixtures
are unavailable.

## Phase 2 97% UI boundary

The final operational slice reuses canonical Reminder, Vendor Activity, restriction, Favorite, and
Order Assistance presentations with existing responsive/focus contracts. No populated browser claim
is added: the local API and disposable Viewer fixtures remain unavailable, which is part of the final
incomplete evidence gate.

## Phase 2 final browser attempt

At `2026-06-21T03:31:08+03:00`, the production preview responded with HTTP 200 on
`http://127.0.0.1:4173`. The in-app browser automation kernel could not start on two attempts because
Windows denied process creation (`CreateProcessAsUserW failed: 5`), so no new route, focus, viewport,
or Viewer assertions are claimed. The populated real-route UI matrix remains incomplete.
