
CREATE TABLE basic_table (
	id uuid NOT NULL PRIMARY KEY,
	comment text
);

CREATE TABLE datetime_table (
	id serial NOT NULL PRIMARY KEY,
	local_timestamp timestamptz,
	utc_timestamp timestamp
);
