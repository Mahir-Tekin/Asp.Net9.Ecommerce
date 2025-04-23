# E-Commerce Database Design

## Core Entities

### 1. Products
- `Id` (Guid)
- `Name` (string)
- `Description` (string)
- `Price` (decimal)
- `StockQuantity` (int)
- `SKU` (string, unique)
- `CategoryId` (Guid, FK)
- `BrandId` (Guid, FK)
- `IsActive` (bool)
- Audit fields from BaseEntity

### 2. Categories
- `Id` (Guid)
- `Name` (string)
- `Description` (string)
- `ParentCategoryId` (Guid?, FK to self)
- `Slug` (string, unique)
- `IsActive` (bool)
- Audit fields from BaseEntity

### 3. Orders
- `Id` (Guid)
- `UserId` (Guid, FK)
- `OrderNumber` (string, unique)
- `OrderDate` (DateTime)
- `Status` (enum: Pending, Processing, Shipped, Delivered, Cancelled)
- `TotalAmount` (decimal)
- `ShippingAddressId` (Guid, FK)
- `BillingAddressId` (Guid, FK)
- `PaymentStatus` (enum: Pending, Paid, Failed, Refunded)
- Audit fields from BaseEntity

### 4. OrderItems
- `Id` (Guid)
- `OrderId` (Guid, FK)
- `ProductId` (Guid, FK)
- `Quantity` (int)
- `UnitPrice` (decimal)
- `TotalPrice` (decimal)
- Audit fields from BaseEntity

### 5. Addresses
- `Id` (Guid)
- `UserId` (Guid, FK)
- `Type` (enum: Shipping, Billing)
- `FirstName` (string)
- `LastName` (string)
- `Street` (string)
- `City` (string)
- `State` (string)
- `Country` (string)
- `PostalCode` (string)
- `Phone` (string)
- `IsDefault` (bool)
- Audit fields from BaseEntity

### 6. Cart
- `Id` (Guid)
- `UserId` (Guid, FK)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

### 7. CartItems
- `Id` (Guid)
- `CartId` (Guid, FK)
- `ProductId` (Guid, FK)
- `Quantity` (int)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

## Key Relationships

1. **Product - Category**
   - One-to-Many: A Category can have multiple Products
   - A Product belongs to one Category

2. **Order - OrderItems**
   - One-to-Many: An Order has multiple OrderItems
   - An OrderItem belongs to one Order

3. **User - Orders**
   - One-to-Many: A User can have multiple Orders
   - An Order belongs to one User

4. **User - Addresses**
   - One-to-Many: A User can have multiple Addresses
   - An Address belongs to one User

5. **User - Cart**
   - One-to-One: A User has one active Cart
   - A Cart belongs to one User

## Indexes

1. **Products**
   - SKU (unique)
   - CategoryId
   - Name (for search)

2. **Orders**
   - OrderNumber (unique)
   - UserId
   - OrderDate

3. **Categories**
   - Slug (unique)
   - ParentCategoryId

## Notes

1. **Soft Delete**
   - All entities inherit from BaseEntity which includes DeletedAt for soft delete

2. **Audit Trail**
   - All entities track CreatedAt and UpdatedAt timestamps

3. **Performance Considerations**
   - Indexes on frequently queried fields
   - Appropriate foreign key relationships
   - Normalized design for data integrity 