-- Create the test table
CREATE TABLE sample_test_table
(
	ID    SERIAL PRIMARY KEY,			  -- Primary Key with auto-increment
	Name  VARCHAR(100) NOT NULL,          -- A VARCHAR column for names
	Age   INT,                            -- An INT column for age
	Email VARCHAR(150)                    -- A VARCHAR column for email addresses
);

-- Insert sample data into the test table
INSERT INTO sample_test_table (Name, Age, Email)
VALUES ('Alice Johnson', 28, 'alice.johnson@example.com'),
	   ('Bob Smith', 34, 'bob.smith@example.com'),
	   ('Clara Green', 29, 'clara.green@example.com');
