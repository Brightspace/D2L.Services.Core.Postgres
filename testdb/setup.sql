
CREATE TABLE basic_table (
	id uuid NOT NULL PRIMARY KEY,
	comment text
);

CREATE TABLE datetime_table (
	id serial NOT NULL PRIMARY KEY,
	local_timestamp timestamptz,
	utc_timestamp timestamp
);

CREATE TABLE array_table (
	id serial NOT NULL PRIMARY KEY,
	guid_array uuid[],
	string_array text[]
);

CREATE TABLE datetime_array_table (
	id serial NOT NULL PRIMARY KEY,
	utc_timestamps timestamp[],
	local_timestamps timestamptz[]
);

CREATE TABLE guid_table (
	id serial NOT NULL PRIMARY KEY,
	guid uuid
);
