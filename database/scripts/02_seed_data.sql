-- NET Building Management System - Seed Data
-- PostgreSQL Database Seed Data

-- Insert sample tenant
INSERT INTO tenants (id, name, subdomain, is_active, subscription_plan, max_buildings, max_units, max_residents, created_by, updated_by)
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'Sample Building Management', 'sample', true, 'Premium', 5, 200, 500, 'System', 'System'),
    ('22222222-2222-2222-2222-222222222222', 'Demo Property Group', 'demo', true, 'Basic', 1, 50, 100, 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample roles
INSERT INTO roles (id, tenant_id, name, description, permissions, created_by, updated_by)
VALUES 
    ('aa111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'SuperAdmin', 'Super Administrator with full access', 
     ARRAY['all'], 'System', 'System'),
    ('bb111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'Admin', 'Building Administrator', 
     ARRAY['building_admin', 'residents_manage', 'payments_manage', 'maintenance_manage'], 'System', 'System'),
    ('cc111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'Manager', 'Building Manager', 
     ARRAY['building_manager', 'residents_view', 'payments_view', 'maintenance_view'], 'System', 'System'),
    ('dd111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'Maintenance', 'Maintenance Staff', 
     ARRAY['maintenance'], 'System', 'System'),
    ('ee111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'Resident', 'Building Resident', 
     ARRAY['resident'], 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample buildings
INSERT INTO buildings (id, tenant_id, name, address, city, country, postal_code, phone, email, description, total_units, construction_year, monthly_maintenance_fee, created_by, updated_by)
VALUES 
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'Sunrise Towers', '123 Main Street', 'Istanbul', 'Turkey', '34000', '+90-212-555-0001', 'info@sunrise-towers.com', 'Modern residential building with 24/7 security', 20, 2020, 500.00, 'System', 'System'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '11111111-1111-1111-1111-111111111111', 'Garden View Residences', '456 Oak Avenue', 'Ankara', 'Turkey', '06000', '+90-312-555-0002', 'info@gardenview.com', 'Family-friendly apartment complex with garden', 15, 2018, 400.00, 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample building units
INSERT INTO building_units (id, tenant_id, building_id, unit_number, unit_type, floor_number, area_sqm, bedrooms, bathrooms, balcony, parking_space, monthly_rent, monthly_maintenance_fee, is_occupied, created_by, updated_by)
VALUES 
    ('11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'A101', 1, 1, 85.50, 2, 1, true, true, 2500.00, 500.00, true, 'System', 'System'),
    ('22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'A102', 1, 1, 95.00, 3, 2, true, true, 3000.00, 500.00, true, 'System', 'System'),
    ('33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'A201', 1, 2, 85.50, 2, 1, true, false, 2300.00, 500.00, false, 'System', 'System'),
    ('44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'A202', 1, 2, 95.00, 3, 2, true, false, 2800.00, 500.00, false, 'System', 'System'),
    ('11111111-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '11111111-1111-1111-1111-111111111111', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'B101', 1, 1, 75.00, 2, 1, true, true, 2000.00, 400.00, true, 'System', 'System'),
    ('22222222-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '11111111-1111-1111-1111-111111111111', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'B102', 1, 1, 80.00, 2, 1, true, true, 2200.00, 400.00, false, 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample residents
INSERT INTO residents (id, tenant_id, first_name, last_name, email, phone, national_id, date_of_birth, occupation, emergency_contact_name, emergency_contact_phone, move_in_date, is_active, created_by, updated_by)
VALUES 
    ('aaaaaaaa-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'Ahmet', 'Yılmaz', 'ahmet.yilmaz@email.com', '+90-532-555-0001', '12345678901', '1985-05-15', 'Software Engineer', 'Fatma Yılmaz', '+90-532-555-0002', '2023-01-01', true, 'System', 'System'),
    ('bbbbbbbb-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'Mehmet', 'Kaya', 'mehmet.kaya@email.com', '+90-532-555-0003', '12345678902', '1990-08-20', 'Doctor', 'Ayşe Kaya', '+90-532-555-0004', '2023-02-15', true, 'System', 'System'),
    ('cccccccc-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'Zeynep', 'Demir', 'zeynep.demir@email.com', '+90-532-555-0005', '12345678903', '1988-12-10', 'Teacher', 'Ali Demir', '+90-532-555-0006', '2023-03-01', true, 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample resident-unit relationships
INSERT INTO resident_units (id, tenant_id, resident_id, building_unit_id, is_primary_resident, move_in_date, deposit_amount, rent_amount, is_active, created_by, updated_by)
VALUES 
    ('11111111-1111-1111-1111-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-1111-1111-1111-111111111111', '11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa', true, '2023-01-01', 5000.00, 2500.00, true, 'System', 'System'),
    ('22222222-1111-1111-1111-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'bbbbbbbb-1111-1111-1111-111111111111', '22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa', true, '2023-02-15', 6000.00, 3000.00, true, 'System', 'System'),
    ('33333333-1111-1111-1111-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'cccccccc-1111-1111-1111-111111111111', '11111111-bbbb-bbbb-bbbb-bbbbbbbbbbbb', true, '2023-03-01', 4000.00, 2000.00, true, 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample users (passwords are hashed using BCrypt for "password123")
INSERT INTO users (id, tenant_id, building_id, resident_id, first_name, last_name, email, password_hash, phone, role, is_active, email_confirmed, created_by, updated_by)
VALUES 
    ('11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', null, null, 'Super', 'Admin', 'superadmin@sample.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewGUhCbqrp6kOe5.', '+90-532-555-9999', 1, true, true, 'System', 'System'),
    ('22222222-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', null, 'Building', 'Admin', 'admin@sample.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewGUhCbqrp6kOe5.', '+90-532-555-9998', 2, true, true, 'System', 'System'),
    ('33333333-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', null, 'Building', 'Manager', 'manager@sample.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewGUhCbqrp6kOe5.', '+90-532-555-9997', 3, true, true, 'System', 'System'),
    ('44444444-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', null, 'Maintenance', 'Staff', 'maintenance@sample.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewGUhCbqrp6kOe5.', '+90-532-555-9996', 4, true, true, 'System', 'System'),
    ('aaaaaaaa-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'aaaaaaaa-1111-1111-1111-111111111111', 'Ahmet', 'Yılmaz', 'ahmet.yilmaz@email.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewGUhCbqrp6kOe5.', '+90-532-555-0001', 5, true, true, 'System', 'System'),
    ('bbbbbbbb-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'bbbbbbbb-1111-1111-1111-111111111111', 'Mehmet', 'Kaya', 'mehmet.kaya@email.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewGUhCbqrp6kOe5.', '+90-532-555-0003', 5, true, true, 'System', 'System'),
    ('cccccccc-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'cccccccc-1111-1111-1111-111111111111', 'Zeynep', 'Demir', 'zeynep.demir@email.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewGUhCbqrp6kOe5.', '+90-532-555-0005', 5, true, true, 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample invoices
INSERT INTO invoices (id, tenant_id, building_unit_id, resident_id, invoice_number, invoice_date, due_date, amount, paid_amount, remaining_amount, status, description, created_by, updated_by)
VALUES 
    ('11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'aaaaaaaa-1111-1111-1111-111111111111', 'INV-2024-001', '2024-01-01', '2024-01-31', 3000.00, 3000.00, 0.00, 2, 'Monthly rent and maintenance fee', 'System', 'System'),
    ('22222222-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'bbbbbbbb-1111-1111-1111-111111111111', 'INV-2024-002', '2024-01-01', '2024-01-31', 3500.00, 3500.00, 0.00, 2, 'Monthly rent and maintenance fee', 'System', 'System'),
    ('33333333-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '11111111-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'cccccccc-1111-1111-1111-111111111111', 'INV-2024-003', '2024-01-01', '2024-01-31', 2400.00, 2400.00, 0.00, 2, 'Monthly rent and maintenance fee', 'System', 'System'),
    ('44444444-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'aaaaaaaa-1111-1111-1111-111111111111', 'INV-2024-004', '2024-02-01', '2024-02-29', 3000.00, 0.00, 3000.00, 1, 'Monthly rent and maintenance fee', 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample payments
INSERT INTO payments (id, tenant_id, building_unit_id, resident_id, invoice_id, payment_date, amount, payment_method, reference_number, status, created_by, updated_by)
VALUES 
    ('11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'aaaaaaaa-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '2024-01-15', 3000.00, 2, 'TRF-2024-001', 2, 'System', 'System'),
    ('22222222-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'bbbbbbbb-1111-1111-1111-111111111111', '22222222-1111-1111-1111-111111111111', '2024-01-20', 3500.00, 2, 'TRF-2024-002', 2, 'System', 'System'),
    ('33333333-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '11111111-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'cccccccc-1111-1111-1111-111111111111', '33333333-1111-1111-1111-111111111111', '2024-01-25', 2400.00, 1, 'CASH-2024-001', 2, 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample expenses
INSERT INTO expenses (id, tenant_id, building_id, category, description, amount, expense_date, vendor_name, invoice_number, approved_by, approved_at, created_by, updated_by)
VALUES 
    ('11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 1, 'Elevator maintenance', 1500.00, '2024-01-15', 'Elevator Service Co.', 'ES-2024-001', '22222222-1111-1111-1111-111111111111', '2024-01-16 10:00:00', 'System', 'System'),
    ('22222222-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 2, 'Electricity bill', 2300.00, '2024-01-20', 'City Electric Company', 'ELEC-2024-001', '22222222-1111-1111-1111-111111111111', '2024-01-21 14:30:00', 'System', 'System'),
    ('33333333-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 4, 'Garden maintenance', 800.00, '2024-01-25', 'Garden Care Services', 'GCS-2024-001', '22222222-1111-1111-1111-111111111111', '2024-01-26 09:15:00', 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Insert sample maintenance requests
INSERT INTO maintenance_requests (id, tenant_id, building_id, building_unit_id, resident_id, title, description, priority, status, category, reported_date, assigned_to_user_id, assigned_date, created_by, updated_by)
VALUES 
    ('11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'aaaaaaaa-1111-1111-1111-111111111111', 'Leaky faucet in kitchen', 'The kitchen faucet is dripping continuously. Please fix it as soon as possible.', 2, 2, 'Plumbing', '2024-01-10 09:00:00', '44444444-1111-1111-1111-111111111111', '2024-01-10 10:00:00', 'System', 'System'),
    ('22222222-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'bbbbbbbb-1111-1111-1111-111111111111', 'Heating system not working', 'The heating system in our unit is not working properly. Temperature is very low.', 3, 1, 'HVAC', '2024-01-15 16:30:00', null, null, 'System', 'System'),
    ('33333333-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '11111111-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'cccccccc-1111-1111-1111-111111111111', 'Broken window lock', 'The window lock in the living room is broken and cannot be secured.', 2, 3, 'Windows', '2024-01-08 14:20:00', '44444444-1111-1111-1111-111111111111', '2024-01-08 15:00:00', 'System', 'System')
ON CONFLICT (id) DO NOTHING;

-- Update building unit occupancy status
UPDATE building_units SET is_occupied = true WHERE id IN ('11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-bbbb-bbbb-bbbb-bbbbbbbbbbbb');

-- Update building total units
UPDATE buildings SET total_units = (SELECT COUNT(*) FROM building_units WHERE building_id = buildings.id);

-- Create a function to update timestamps
CREATE OR REPLACE FUNCTION update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create triggers to automatically update timestamps
CREATE TRIGGER update_tenants_timestamp BEFORE UPDATE ON tenants FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_buildings_timestamp BEFORE UPDATE ON buildings FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_building_units_timestamp BEFORE UPDATE ON building_units FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_residents_timestamp BEFORE UPDATE ON residents FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_resident_units_timestamp BEFORE UPDATE ON resident_units FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_users_timestamp BEFORE UPDATE ON users FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_roles_timestamp BEFORE UPDATE ON roles FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_user_roles_timestamp BEFORE UPDATE ON user_roles FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_invoices_timestamp BEFORE UPDATE ON invoices FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_payments_timestamp BEFORE UPDATE ON payments FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_expenses_timestamp BEFORE UPDATE ON expenses FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER update_maintenance_requests_timestamp BEFORE UPDATE ON maintenance_requests FOR EACH ROW EXECUTE FUNCTION update_timestamp();