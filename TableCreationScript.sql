CREATE TABLE manager (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP NOT NULL,
    modified_at TIMESTAMP NOT NULL,
    department INTEGER,
    departmental_id TEXT,
    country_id INTEGER
);



CREATE TABLE employee (
    id SERIAL PRIMARY KEY,
    manager_id INTEGER NOT NULL
        constraint fk_employee_manager
            references manager
            on delete restrict,
    created_at TIMESTAMP NOT NULL,
    modified_at TIMESTAMP NOT NULL,
    location_id INTEGER,
    building_id INTEGER,
    original_building_id INTEGER,
    provider_id INTEGER,
    provider_settings_id INTEGER,
    status INTEGER NOT NULL,
    eligible_for_bonus BOOLEAN NOT NULL
);