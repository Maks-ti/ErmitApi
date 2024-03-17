
CREATE TABLE users (
	id uuid NOT NULL,
	login varchar(256) NOT NULL,
	"name" varchar NOT NULL,
	is_admin bool NOT NULL DEFAULT FALSE,
	password_hash varchar NOT NULL,
	salt varchar NOT NULL,
	CONSTRAINT pk_users PRIMARY KEY (id)
);

CREATE TABLE "location" (
	id serial NOT NULL,
	"name" varchar NOT NULL,
	description TEXT NOT NULL,
	picture_data bytea NULL,
	picture_extension varchar NULL,
	CONSTRAINT pk_location PRIMARY KEY (id)
);

CREATE TABLE showplace (
	id serial NOT NULL,
	"name" varchar NOT NULL,
	description TEXT NOT NULL DEFAULT '',
	address TEXT NOT NULL,
	location_id int NOT NULL,
	picture_data bytea NULL,
	picture_extension varchar NULL,
	CONSTRAINT pk_showplace PRIMARY KEY (id),
	CONSTRAINT fk_showplace__location FOREIGN KEY (location_id) REFERENCES "location"(id)
);

CREATE TABLE achievement (
	id serial NOT NULL,
	"name" varchar NOT NULL,
	description TEXT NULL,
	location_id int NULL,
	showplace_id int NULL,	
	picture_data bytea NULL,
	picture_extension varchar NULL,
	CONSTRAINT pk_achievment PRIMARY KEY (id),
	CONSTRAINT fk_achievement__location FOREIGN KEY (location_id) REFERENCES "location"(id),
	CONSTRAINT fk_achievement__showplace FOREIGN KEY (showplace_id) REFERENCES showplace(id),
	CONSTRAINT achievement_only_one_foreign_key CHECK (NOT (location_id IS NOT NULL AND showplace_id IS NOT NULL))
);

CREATE TABLE user_achievement (
	id serial NOT NULL,
	user_id uuid NOT NULL,
	achievement_id int NOT NULL,
	CONSTRAINT pk_user_achivement PRIMARY KEY (id),
	CONSTRAINT unique_user_achievement UNIQUE (user_id, achievement_id),
	CONSTRAINT fk_user_achievement__users FOREIGN KEY (user_id) REFERENCES users(id),
	CONSTRAINT fk_user_achievement__achievement FOREIGN KEY (achievement_id) REFERENCES achievement(id)
);

CREATE TABLE stand (
	id serial NOT NULL,
	"name" varchar NOT NULL,
	location_id int NOT NULL,
	CONSTRAINT pk_stand PRIMARY KEY (id),
	CONSTRAINT kf_stand__location FOREIGN KEY (location_id) REFERENCES "location"(id)
);


DROP TABLE user_achievement;

DROP TABLE users;

DROP TABLE achievement;

DROP TABLE stand;

DROP TABLE showplace;

DROP TABLE "location";

