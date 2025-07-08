
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create tenants table
CREATE TABLE IF NOT EXISTS tenants (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    subdomain VARCHAR(100) NOT NULL UNIQUE,
    is_active BOOLEAN DEFAULT TRUE,
    subscription_plan VARCHAR(50) DEFAULT 'Basic',
    subscription_expires_at TIMESTAMP,
    max_buildings INTEGER DEFAULT 1,
    max_units INTEGER DEFAULT 50,
    max_residents INTEGER DEFAULT 100,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create buildings table
CREATE TABLE IF NOT EXISTS buildings (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    name VARCHAR(255) NOT NULL,
    address TEXT NOT NULL,
    city VARCHAR(100) NOT NULL,
    country VARCHAR(100) NOT NULL,
    postal_code VARCHAR(20),
    phone VARCHAR(20),
    email VARCHAR(255),
    description TEXT,
    total_units INTEGER DEFAULT 0,
    construction_year INTEGER,
    monthly_maintenance_fee DECIMAL(10,2) DEFAULT 0.00,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create building_units table
CREATE TABLE IF NOT EXISTS building_units (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    building_id UUID NOT NULL REFERENCES buildings(id) ON DELETE RESTRICT,
    unit_number VARCHAR(10) NOT NULL,
    unit_type INTEGER NOT NULL DEFAULT 1, -- 1: Apartment, 2: Commercial, 3: Parking, 4: Storage
    floor_number INTEGER,
    area_sqm DECIMAL(8,2),
    bedrooms INTEGER DEFAULT 0,
    bathrooms INTEGER DEFAULT 0,
    balcony BOOLEAN DEFAULT FALSE,
    parking_space BOOLEAN DEFAULT FALSE,
    monthly_rent DECIMAL(10,2) DEFAULT 0.00,
    monthly_maintenance_fee DECIMAL(10,2) DEFAULT 0.00,
    is_occupied BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE,
    UNIQUE(building_id, unit_number)
);

-- Create residents table
CREATE TABLE IF NOT EXISTS residents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    national_id VARCHAR(50),
    date_of_birth DATE,
    occupation VARCHAR(100),
    emergency_contact_name VARCHAR(200),
    emergency_contact_phone VARCHAR(20),
    move_in_date DATE,
    move_out_date DATE,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create resident_units table (many-to-many relationship)
CREATE TABLE IF NOT EXISTS resident_units (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    resident_id UUID NOT NULL REFERENCES residents(id) ON DELETE RESTRICT,
    building_unit_id UUID NOT NULL REFERENCES building_units(id) ON DELETE RESTRICT,
    is_primary_resident BOOLEAN DEFAULT FALSE,
    move_in_date DATE NOT NULL,
    move_out_date DATE,
    deposit_amount DECIMAL(10,2) DEFAULT 0.00,
    rent_amount DECIMAL(10,2) DEFAULT 0.00,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create roles table
CREATE TABLE IF NOT EXISTS roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    permissions TEXT[], -- Array of permission strings
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create users table
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    building_id UUID REFERENCES buildings(id) ON DELETE RESTRICT,
    resident_id UUID REFERENCES residents(id) ON DELETE RESTRICT,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    role INTEGER NOT NULL DEFAULT 5, -- 1: SuperAdmin, 2: Admin, 3: Manager, 4: Maintenance, 5: Resident
    is_active BOOLEAN DEFAULT TRUE,
    email_confirmed BOOLEAN DEFAULT FALSE,
    last_login_date TIMESTAMP,
    photo_url VARCHAR(500),
    refresh_token VARCHAR(500),
    refresh_token_expiry_time TIMESTAMP,
    two_factor_enabled BOOLEAN DEFAULT FALSE,
    security_stamp VARCHAR(255),
    access_failed_count INTEGER DEFAULT 0,
    lockout_end TIMESTAMP,
    lockout_enabled BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create user_roles table (many-to-many relationship)
CREATE TABLE IF NOT EXISTS user_roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    role_id UUID NOT NULL REFERENCES roles(id) ON DELETE RESTRICT,
    assigned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    assigned_by VARCHAR(255) DEFAULT 'System',
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create invoices table
CREATE TABLE IF NOT EXISTS invoices (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    building_unit_id UUID NOT NULL REFERENCES building_units(id) ON DELETE RESTRICT,
    resident_id UUID NOT NULL REFERENCES residents(id) ON DELETE RESTRICT,
    invoice_number VARCHAR(50) NOT NULL UNIQUE,
    invoice_date DATE NOT NULL,
    due_date DATE NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    paid_amount DECIMAL(10,2) DEFAULT 0.00,
    remaining_amount DECIMAL(10,2) NOT NULL,
    status INTEGER NOT NULL DEFAULT 1, -- 1: Pending, 2: Paid, 3: Overdue, 4: Cancelled
    description TEXT,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create payments table
CREATE TABLE IF NOT EXISTS payments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    building_unit_id UUID NOT NULL REFERENCES building_units(id) ON DELETE RESTRICT,
    resident_id UUID NOT NULL REFERENCES residents(id) ON DELETE RESTRICT,
    invoice_id UUID REFERENCES invoices(id) ON DELETE RESTRICT,
    payment_date DATE NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    payment_method INTEGER NOT NULL DEFAULT 1, -- 1: Cash, 2: BankTransfer, 3: CreditCard, 4: Check
    reference_number VARCHAR(100),
    status INTEGER NOT NULL DEFAULT 1, -- 1: Pending, 2: Completed, 3: Failed, 4: Cancelled
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create expenses table
CREATE TABLE IF NOT EXISTS expenses (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    building_id UUID NOT NULL REFERENCES buildings(id) ON DELETE RESTRICT,
    category INTEGER NOT NULL DEFAULT 1, -- 1: Maintenance, 2: Utilities, 3: Security, 4: Cleaning, 5: Administration, 6: Other
    description TEXT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    expense_date DATE NOT NULL,
    vendor_name VARCHAR(255),
    invoice_number VARCHAR(100),
    receipt_url VARCHAR(500),
    approved_by UUID REFERENCES users(id) ON DELETE RESTRICT,
    approved_at TIMESTAMP,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create maintenance_requests table
CREATE TABLE IF NOT EXISTS maintenance_requests (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE RESTRICT,
    building_id UUID NOT NULL REFERENCES buildings(id) ON DELETE RESTRICT,
    building_unit_id UUID REFERENCES building_units(id) ON DELETE RESTRICT,
    resident_id UUID NOT NULL REFERENCES residents(id) ON DELETE RESTRICT,
    title VARCHAR(255) NOT NULL,
    description TEXT NOT NULL,
    priority INTEGER NOT NULL DEFAULT 2, -- 1: Low, 2: Medium, 3: High, 4: Critical
    status INTEGER NOT NULL DEFAULT 1, -- 1: Pending, 2: InProgress, 3: Completed, 4: Cancelled
    category VARCHAR(100),
    reported_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    assigned_to_user_id UUID REFERENCES users(id) ON DELETE RESTRICT,
    assigned_date TIMESTAMP,
    scheduled_date TIMESTAMP,
    completed_date TIMESTAMP,
    estimated_cost DECIMAL(10,2),
    actual_cost DECIMAL(10,2),
    approved_by UUID REFERENCES users(id) ON DELETE RESTRICT,
    approved_at TIMESTAMP,
    notes TEXT,
    photos TEXT[], -- Array of photo URLs
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) DEFAULT 'System',
    updated_by VARCHAR(255) DEFAULT 'System',
    is_deleted BOOLEAN DEFAULT FALSE
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_tenants_subdomain ON tenants(subdomain);
CREATE INDEX IF NOT EXISTS idx_tenants_is_active ON tenants(is_active);

CREATE INDEX IF NOT EXISTS idx_buildings_tenant_id ON buildings(tenant_id);
CREATE INDEX IF NOT EXISTS idx_buildings_name ON buildings(tenant_id, name);

CREATE INDEX IF NOT EXISTS idx_building_units_building_id ON building_units(building_id);
CREATE INDEX IF NOT EXISTS idx_building_units_unit_number ON building_units(building_id, unit_number);
CREATE INDEX IF NOT EXISTS idx_building_units_occupied ON building_units(building_id, is_occupied);

CREATE INDEX IF NOT EXISTS idx_residents_tenant_id ON residents(tenant_id);
CREATE INDEX IF NOT EXISTS idx_residents_email ON residents(tenant_id, email);
CREATE INDEX IF NOT EXISTS idx_residents_active ON residents(tenant_id, is_active);

CREATE INDEX IF NOT EXISTS idx_resident_units_resident_id ON resident_units(resident_id);
CREATE INDEX IF NOT EXISTS idx_resident_units_building_unit_id ON resident_units(building_unit_id);
CREATE INDEX IF NOT EXISTS idx_resident_units_active ON resident_units(is_active);

CREATE INDEX IF NOT EXISTS idx_users_tenant_id ON users(tenant_id);
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_building_id ON users(building_id);
CREATE INDEX IF NOT EXISTS idx_users_role ON users(tenant_id, role);

CREATE INDEX IF NOT EXISTS idx_user_roles_user_id ON user_roles(user_id);
CREATE INDEX IF NOT EXISTS idx_user_roles_role_id ON user_roles(role_id);

CREATE INDEX IF NOT EXISTS idx_invoices_tenant_id ON invoices(tenant_id);
CREATE INDEX IF NOT EXISTS idx_invoices_building_unit_id ON invoices(building_unit_id);
CREATE INDEX IF NOT EXISTS idx_invoices_resident_id ON invoices(resident_id);
CREATE INDEX IF NOT EXISTS idx_invoices_status ON invoices(tenant_id, status);
CREATE INDEX IF NOT EXISTS idx_invoices_due_date ON invoices(tenant_id, due_date);
CREATE INDEX IF NOT EXISTS idx_invoices_number ON invoices(invoice_number);

CREATE INDEX IF NOT EXISTS idx_payments_tenant_id ON payments(tenant_id);
CREATE INDEX IF NOT EXISTS idx_payments_building_unit_id ON payments(building_unit_id);
CREATE INDEX IF NOT EXISTS idx_payments_resident_id ON payments(resident_id);
CREATE INDEX IF NOT EXISTS idx_payments_invoice_id ON payments(invoice_id);
CREATE INDEX IF NOT EXISTS idx_payments_date ON payments(tenant_id, payment_date);
CREATE INDEX IF NOT EXISTS idx_payments_reference ON payments(reference_number);

CREATE INDEX IF NOT EXISTS idx_expenses_tenant_id ON expenses(tenant_id);
CREATE INDEX IF NOT EXISTS idx_expenses_building_id ON expenses(building_id);
CREATE INDEX IF NOT EXISTS idx_expenses_category ON expenses(tenant_id, category);
CREATE INDEX IF NOT EXISTS idx_expenses_date ON expenses(tenant_id, expense_date);

CREATE INDEX IF NOT EXISTS idx_maintenance_requests_tenant_id ON maintenance_requests(tenant_id);
CREATE INDEX IF NOT EXISTS idx_maintenance_requests_building_id ON maintenance_requests(building_id);
CREATE INDEX IF NOT EXISTS idx_maintenance_requests_building_unit_id ON maintenance_requests(building_unit_id);
CREATE INDEX IF NOT EXISTS idx_maintenance_requests_resident_id ON maintenance_requests(resident_id);
CREATE INDEX IF NOT EXISTS idx_maintenance_requests_assigned_to ON maintenance_requests(assigned_to_user_id);
CREATE INDEX IF NOT EXISTS idx_maintenance_requests_status ON maintenance_requests(tenant_id, status);
CREATE INDEX IF NOT EXISTS idx_maintenance_requests_priority ON maintenance_requests(tenant_id, priority);