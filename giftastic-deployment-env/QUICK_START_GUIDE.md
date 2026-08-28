# Quick Start Guide - New Features

## 🚀 All Three Features Are Ready!

### What's New:
1. **Product Recommendations** - "Most Frequently Bought" and "What Others Are Buying"
2. **Vendor Analytics Dashboard** - Complete sales and performance metrics
3. **Vendor Application System** - Users can apply to become vendors

---

## Starting the Application

### 1. Start Backend (Already Running)
The backend server is currently running on port 8080.

If you need to restart:
```bash
cd b:\giftastic-gp
mvnw.cmd spring-boot:run
```

### 2. Start Frontend
```bash
cd b:\giftastic-gp\frontend
npm run dev
```

The frontend will start on `http://localhost:5173`

---

## Testing the New Features

### Feature 1: Product Recommendations

**Where to find it:**
- Homepage (scroll down to bottom)

**What you'll see:**
- "Most Frequently Bought" section
- "What Others Are Buying" section
- Product cards with images, prices, and ratings

**Note:** Sections will be empty if no orders exist yet. To populate:
1. Create some products as a vendor
2. Place orders as customers
3. Recommendations will appear based on order data

---

### Feature 2: Vendor Analytics Dashboard

**How to access:**
1. Login as a vendor account
2. Click "Analytics" in the vendor sidebar
3. View your store's performance metrics

**What you'll see:**
- Overview cards (products, orders, revenue, reviews)
- Top performing products table
- Revenue by month
- Order status breakdown
- Date range filter

**Test with:**
- Vendor account that has products and orders
- Try filtering by date range

---

### Feature 3: Vendor Application System

#### For Users (Applying to Become Vendor):

**Step 1: Submit Application**
1. Scroll to footer, click "Become a Vendor"
2. Fill out the application form:
   - Store Name (required)
   - Description
   - Logo and Banner URLs
   - Contact information
   - Social media links
   - Working hours
3. Click "Submit Application"

**Step 2: Check Application Status**
1. Navigate to `/my-vendor-applications`
2. View your application status:
   - **PENDING**: Awaiting admin review
   - **APPROVED**: Vendor account created!
   - **REJECTED**: See rejection reason

#### For Admins (Reviewing Applications):

**Step 1: View Pending Applications**
1. Login as admin
2. Navigate to `/admin/vendor-applications`
3. See all pending applications

**Step 2: Review Application**
1. Click "Review" button on any application
2. Choose action:
   - **Approve**: Creates vendor account automatically
   - **Reject**: Provide reason for rejection
3. Submit review

**What happens on approval:**
- Vendor account is created
- User can now access vendor features
- User can start adding products

---

## Quick Test Scenarios

### Scenario 1: Complete Vendor Application Flow
```
1. Register new user account
2. Click "Become a Vendor" in footer
3. Fill out application form
4. Submit application
5. Login as admin
6. Go to /admin/vendor-applications
7. Review and approve application
8. Logout and login as the new vendor
9. Access vendor dashboard
10. View analytics (will be empty initially)
```

### Scenario 2: View Recommendations
```
1. Ensure some products exist
2. Place a few orders
3. Visit homepage
4. Scroll to bottom
5. See "Most Frequently Bought" products
6. See "What Others Are Buying" products
```

### Scenario 3: Vendor Analytics
```
1. Login as vendor with existing products
2. Ensure some orders exist for your products
3. Click "Analytics" in sidebar
4. View overview metrics
5. Check top products table
6. Try date range filter
7. View revenue history
```

---

## API Endpoints Quick Reference

### Recommendations (Public):
```
GET /api/v1/recommendations/most-frequently-bought?limit=10
GET /api/v1/recommendations/what-others-are-buying?limit=10
```

### Analytics (Vendor/Admin):
```
GET /api/v1/vendors/{supplierId}/analytics
GET /api/v1/vendors/{supplierId}/analytics?startDate=2024-01-01T00:00:00&endDate=2024-12-31T23:59:59
```

### Vendor Applications:
```
POST   /api/v1/vendor-applications                    (User: Submit)
GET    /api/v1/vendor-applications/my-applications    (User: View own)
GET    /api/v1/vendor-applications/pending            (Admin: View pending)
GET    /api/v1/vendor-applications/{id}               (Owner/Admin: View)
PATCH  /api/v1/vendor-applications/{id}/review        (Admin: Approve/Reject)
```

---

## Testing with cURL

### Test Recommendations:
```bash
curl "http://localhost:8080/api/v1/recommendations/most-frequently-bought?limit=5"
curl "http://localhost:8080/api/v1/recommendations/what-others-are-buying?limit=5"
```

### Test Analytics (requires auth token):
```bash
curl -H "Authorization: Bearer YOUR_TOKEN" \
  "http://localhost:8080/api/v1/vendors/YOUR_SUPPLIER_ID/analytics"
```

### Test Submit Application (requires auth token):
```bash
curl -X POST \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"storeName":"My Store","description":"Test store","contactEmail":"test@example.com"}' \
  "http://localhost:8080/api/v1/vendor-applications"
```

---

## Troubleshooting

### Recommendations showing empty:
- **Cause**: No orders in database yet
- **Solution**: Create products and place orders first

### Analytics showing zero:
- **Cause**: Vendor has no products or orders
- **Solution**: Add products and wait for orders

### Application submission fails:
- **Possible causes**:
  - User already has vendor account
  - User already has pending application
  - Store name is empty
- **Solution**: Check error message, ensure requirements met

### Admin can't see pending applications:
- **Cause**: No pending applications or insufficient permissions
- **Solution**: Ensure admin has ACTIVATE_VENDORS permission

---

## Next Steps

1. **Test all three features** using the scenarios above
2. **Create sample data** (products, orders) to see recommendations
3. **Submit test applications** to verify workflow
4. **Review analytics** with real vendor data

---

## Support

If you encounter any issues:
1. Check browser console for errors
2. Check backend logs for exceptions
3. Verify authentication tokens are valid
4. Ensure database is running and accessible

---

## Summary

✅ **Backend**: All endpoints working and tested
✅ **Frontend**: All pages created and integrated
✅ **Routes**: All routes configured
✅ **Security**: Permissions properly set
✅ **UI/UX**: Responsive and user-friendly

**Status**: 🎉 **READY FOR USE!**
