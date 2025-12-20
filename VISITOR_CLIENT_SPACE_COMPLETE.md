# Visitor and Client Space Documentation

## Overview
This document describes the implementation of the Visitor (public) and Client spaces in the Solution_Magasin application.

## Architecture

### Database Schema Integration
The implementation correctly uses the existing database schema without modifications:

#### Models Used
- **Article**: Products with properties `NomArt`, `PrixUnit`, `ImagePath`, `ReferenceArt`, `DesignationArt`
- **Stock**: Inventory with `QteDispo` (quantity available)
- **Payment**: Payment records with `Methode`, `EstPaye`, `DatePayment`
- **Vente**: Sales with `TotalV`, `AdresseLiv`, `Status`
- **DetailVente**: Sale items with `QteDv` (quantity), `MontantDv` (amount)
- **Review**: Product reviews with `Rating`, `Comment`
- **Client**: Customer information
- **Categorie**: Product categories

## Visitor Space (Public Access)

### Features

#### 1. Visitor Home Page (`/Visitor/Index`)
- **Hero Section**: Welcome message with call-to-action buttons
- **Categories Section**: Display of 6 main product categories with icons
- **Featured Products**: 8 most popular products (by reviews and date)
- **CTA Section**: Registration and login prompts for non-authenticated users

#### 2. Product Catalog (`/Visitor/Catalog`)
- **Filtering**:
  - By category (dropdown)
  - By search query (product name, description, or reference)
- **Pagination**: 12 products per page
- **Product Cards**: Display:
  - Product image or placeholder
  - Category badge
  - Product name and description (truncated)
  - Price
  - Stock status (in stock/out of stock)
  - Average rating and review count
- **Responsive Layout**: 3 columns on desktop, adapts to mobile

#### 3. Product Details (`/Visitor/ProductDetails/{id}`)
- **Product Information**:
  - Large product image
  - Full product description
  - Price and stock availability
  - Average rating with star display
  - Reference number
- **Add to Cart**: Available for authenticated clients
- **Reviews Section**: Display customer reviews (up to 5)
- **Related Products**: Up to 4 products from the same category

### Controller: `VisitorController`
- **No authentication required**
- Methods:
  - `Index()`: Home page
  - `Catalog(categoryId, search, page)`: Product listing with filters
  - `ProductDetails(id)`: Detailed product view

## Client Space (Authenticated)

### Features

#### 1. Client Dashboard (`/Client/Index`)
- **Welcome Section**: Personalized greeting
- **Quick Access Cards**:
  - Browse Products
  - Shopping Cart
  - My Orders (with count)
  - My Profile
- **Quick Stats**: Delivery info, secure payment, support

#### 2. Shopping Cart (`/Client/Cart`)
- **Cart Display**:
  - Product image, name, reference
  - Unit price and quantity
  - Quantity controls (increment/decrement)
  - Remove item button
- **Cart Summary**:
  - Subtotal
  - Shipping cost (fixed at 50 DH)
  - Total amount
- **Actions**:
  - Continue shopping
  - Clear cart
  - Proceed to checkout
- **AJAX Operations**: Add, update, remove items without page reload

#### 3. Checkout Process

##### 3.1 Checkout Form (`/Client/Checkout`)
- **Shipping Information**:
  - Delivery address (pre-filled from profile)
  - Phone number (pre-filled)
  - Delivery instructions (optional)
- **Order Summary**: Items, quantities, prices, total
- **Validation**: Server-side validation of all fields

##### 3.2 Payment Simulation (`/Client/Payment`)
- **Simulated Payment Form**:
  - Cardholder name
  - Card number (formatted as XXXX XXXX XXXX XXXX)
  - Expiry date (MM/YY format)
  - CVV code
- **Note**: This is a simulation - no real payment processing
- **Client-side Formatting**: Auto-format card number and expiry date
- **Security Notice**: Reassurance that info is not stored

##### 3.3 Order Confirmation (`/Client/OrderConfirmation`)
- **Success Message**: Visual confirmation with order number
- **Order Details**:
  - Order ID and date
  - Payment status (green badge)
  - List of purchased items with images
  - Total amount
- **Actions**: View my orders, continue shopping

#### 4. Order Management

##### 4.1 Order History (`/Client/MyOrders`)
- **Order List**: All customer orders, newest first
- **Each Order Shows**:
  - Order number and date
  - Payment status badge (color-coded)
  - Total amount
  - Number of items
  - Preview of first 3 items with images
- **Actions**: View details button for each order

##### 4.2 Order Details (`/Client/OrderDetails/{id}`)
- **Full Order Information**:
  - Order number, date, status
  - Payment method and status
  - Complete item list with quantities and prices
  - Subtotal, shipping, total
  - Delivery address and phone
- **Actions**: Back to orders, print order

#### 5. Profile Management (`/Client/Profile`)
- **View/Edit Profile**:
  - First name and last name
  - Email address
  - Delivery address
  - Phone number
  - Account creation date (read-only)
- **Synchronization**: Updates both Client and ApplicationUser records

### Controller: `ClientController`
- **Authentication Required**: [Authorize(Policy = RoleConstants.ClientPolicy)]
- **Session Management**: Shopping cart stored in session

#### Main Methods:
1. **Cart Management**:
   - `AddToCart(productId, quantity)`: Add product with stock validation
   - `UpdateCartItem(productId, quantity)`: Modify cart quantities
   - `RemoveFromCart(productId)`: Remove single item
   - `ClearCart()`: Empty entire cart

2. **Checkout & Payment**:
   - `Checkout()`: Display checkout form
   - `Checkout(model)`: Process checkout, create payment record
   - `Payment(paymentId)`: Display payment form
   - `ProcessPayment(model)`: Simulate payment, create Vente and DetailVente records
   - `OrderConfirmation(orderId)`: Show order success

3. **Order Management**:
   - `MyOrders()`: List all customer orders
   - `OrderDetails(id)`: Show specific order details

4. **Profile**:
   - `Profile()`: Display profile form
   - `Profile(model)`: Update profile

## ViewModels

### Product ViewModels
- **ProductCatalogViewModel**: Catalog page with products, categories, pagination
- **ProductCardViewModel**: Individual product card data
- **ProductDetailsViewModel**: Detailed product with reviews and related products
- **ProductReviewViewModel**: Customer review display

### Shopping ViewModels
- **CartViewModel**: Cart with items, subtotal, shipping, total
- **CartItemViewModel**: Single cart item with quantity controls
- **CheckoutViewModel**: Checkout form with cart and shipping info
- **PaymentViewModel**: Payment form fields

### Order ViewModels
- **OrderHistoryViewModel**: List of all orders
- **OrderSummaryViewModel**: Order preview in list
- **OrderDetailsViewModel**: Complete order information
- **OrderItemViewModel**: Single order item

## Session Management

### Shopping Cart
- **Storage**: `HttpContext.Session`
- **Key**: `"ShoppingCart"`
- **Format**: JSON serialization of `CartViewModel`
- **Lifetime**: 30 minutes idle timeout
- **Persistence**: Cleared after successful order

### Configuration (Program.cs)
```csharp
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

## Views Structure

### Visitor Views (`Views/Visitor/`)
- `Index.cshtml`: Home page with hero, categories, featured products
- `Catalog.cshtml`: Product grid with sidebar filters
- `ProductDetails.cshtml`: Detailed product view

### Client Views (`Views/Client/`)
- `Index.cshtml`: Client dashboard
- `Cart.cshtml`: Shopping cart
- `Checkout.cshtml`: Checkout form
- `Payment.cshtml`: Payment simulation form
- `OrderConfirmation.cshtml`: Order success page
- `MyOrders.cshtml`: Order history list
- `OrderDetails.cshtml`: Detailed order view
- `Profile.cshtml`: Profile management (already existing)

## Navigation Updates

### Layout Changes (`Views/Shared/_Layout.cshtml`)
- Added "Catalogue" link visible to all users
- Added "Panier" and "Mes Commandes" links for authenticated clients
- Maintained existing admin and employee navigation

### Home Controller Routing
Updated `HomeController.Index()` to redirect:
- Administrators ? `/Admin/Index`
- Clients ? `/Client/Index`
- Employees ? `/Employee/Index`
- Visitors ? `/Visitor/Index`

## Business Logic

### Stock Management
- **Add to Cart**: Validates available stock (QteDispo)
- **Checkout**: Reserves items (stock not yet deducted)
- **Payment Success**: Deducts from stock (QteDispo -= quantity)
- **Stock Update**: Sets DateModification to current date

### Order Processing Flow
1. **Cart** ? User adds products
2. **Checkout** ? User enters shipping info
   - Creates Payment record (EstPaye = false)
3. **Payment** ? User submits payment form
   - Updates Payment (EstPaye = true)
   - Creates Vente record
   - Creates DetailVente records for each item
   - Updates Stock quantities
   - Clears cart session
4. **Confirmation** ? Display order success

### Payment Simulation
- **No real payment processing**
- **User inputs**:
  - Card details (validated format only)
  - Not stored or transmitted
- **Simulation**: Automatically marks as paid
- **Status**: Payment.EstPaye = true

## Data Calculations

### Cart
- **Subtotal**: Sum of (UnitPrice × Quantity) for all items
- **Shipping**: Fixed 50 DH
- **Total**: Subtotal + Shipping

### Orders
- **TotalV**: Stored in Vente table
- **Item Total**: MontantDv in DetailVente
- **UnitPrice**: MontantDv ÷ QteDv (calculated for display)

## Security Features

### Authorization
- **Visitor routes**: No authentication required
- **Client routes**: `[Authorize(Policy = RoleConstants.ClientPolicy)]`
- **Policy**: Requires "Client" role

### Data Protection
- **Client Isolation**: Clients only see their own orders
- **Query Filters**: `Where(v => v.IdClient == user.ClientId)`
- **Profile Updates**: Only allow self-updates

### Session Security
- **HttpOnly cookies**: Prevents JavaScript access
- **Essential**: Required for application functionality
- **IsEssential = true**: Exempt from GDPR consent

## Styling

### CSS Framework
- **Bootstrap 5**: Responsive grid and components
- **Bootstrap Icons**: Icon library
- **Custom Classes**: `.hover-lift`, `.hover-card` for animations

### Responsive Design
- **Mobile-first**: Stacks columns on small screens
- **Breakpoints**:
  - `col-12`: Mobile (< 576px)
  - `col-md-6`: Tablet (? 768px)
  - `col-lg-3/4`: Desktop (? 992px)

### Color Scheme
- **Primary**: Bootstrap blue (`#0d6efd`)
- **Success**: Green badges for in-stock, paid status
- **Danger**: Red badges for out-of-stock
- **Warning**: Yellow badges for pending status

## AJAX Features

### Cart Operations
All cart modifications use AJAX to avoid page reload:

```javascript
fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body: 'productId=' + id + '&quantity=' + qty
})
.then(response => response.json())
.then(data => {
    if (data.success) {
        // Update UI
    }
});
```

### Operations
- **Add to Cart**: From product details page
- **Update Quantity**: Increment/decrement in cart
- **Remove Item**: Delete from cart
- **Clear Cart**: Empty entire cart

## Performance Optimizations

### Database Queries
- **Eager Loading**: `.Include()` for related data
- **Projections**: `.Select()` to load only needed fields
- **Pagination**: Limits results per page
- **Indexing**: Leverages database indexes on foreign keys

### Session
- **In-Memory Cache**: Fast read/write
- **JSON Serialization**: Compact storage
- **Scoped to User**: Isolated per session

## Testing Checklist

### Visitor Features
- [ ] Browse catalog without authentication
- [ ] Filter by category
- [ ] Search products
- [ ] View product details
- [ ] See related products
- [ ] Pagination works correctly

### Client Features
- [ ] Dashboard displays correctly
- [ ] Add products to cart
- [ ] Update cart quantities
- [ ] Remove cart items
- [ ] Proceed through checkout
- [ ] Submit payment simulation
- [ ] View order confirmation
- [ ] Access order history
- [ ] View order details
- [ ] Update profile

### Edge Cases
- [ ] Empty cart prevents checkout
- [ ] Stock validation prevents over-ordering
- [ ] Maximum quantity enforced
- [ ] Session timeout handled gracefully
- [ ] Client can only access own orders

## Future Enhancements

### Potential Features
1. **Wishlist**: Save products for later
2. **Product Reviews**: Allow clients to review purchased items
3. **Order Tracking**: Status updates (processing, shipped, delivered)
4. **Email Notifications**: Order confirmation, shipping updates
5. **Coupon Codes**: Discount functionality
6. **Multiple Addresses**: Saved shipping addresses
7. **Order History Export**: PDF or CSV download
8. **Real Payment Integration**: Stripe, PayPal, etc.
9. **Product Ratings**: Star rating system
10. **Advanced Search**: Filters by price, rating, availability

### Technical Improvements
1. **Caching**: Redis for session management
2. **Image Optimization**: Thumbnails, lazy loading
3. **Search Engine**: Elasticsearch for better search
4. **API**: RESTful API for mobile app
5. **SignalR**: Real-time stock updates
6. **Unit Tests**: Comprehensive test coverage

## Troubleshooting

### Common Issues

#### Cart is Empty After Adding Items
- **Cause**: Session not configured or expired
- **Solution**: Verify session middleware in Program.cs, check timeout

#### Cannot Add to Cart
- **Cause**: Not authenticated or wrong role
- **Solution**: Ensure user is logged in with "Client" role

#### Stock Not Updating
- **Cause**: Transaction not committed
- **Solution**: Verify `SaveChangesAsync()` is called

#### Orders Not Showing
- **Cause**: Client filter issue
- **Solution**: Check `user.ClientId` is set correctly

## Deployment Notes

### Configuration
- **Session**: Configure timeout for production
- **HTTPS**: Required for secure cookies
- **Connection String**: Update for production database

### Performance
- **Session Storage**: Consider Redis for load-balanced environments
- **Image Storage**: Use CDN for product images
- **Database**: Enable connection pooling

## Support

For issues or questions:
1. Check this documentation
2. Review controller comments
3. Inspect browser console for AJAX errors
4. Check server logs for exceptions
5. Verify database records are correct

---

**Version**: 1.0  
**Last Updated**: December 2024  
**Author**: AZZIOUI Development Team
