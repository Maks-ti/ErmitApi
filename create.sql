CREATE TABLE users (
	id uuid NOT NULL,
	login varchar(256) NOT NULL,
	name varchar NOT NULL,
	is_admin bool NOT NULL DEFAULT FALSE,
	password_hash varchar NOT NULL,
	salt varchar NOT NULL,
	CONSTRAINT pk_users PRIMARY KEY (id)
);


CREATE TABLE achievement (
	id serial NOT NULL,
	name varchar NOT NULL,
	description TEXT NULL,
	CONSTRAINT pk_achivment PRIMARY KEY (id)
);

CREATE TABLE user_achievement (
	id serial NOT NULL,
	user_id uuid NOT NULL,
	achivement_id int NOT NULL,
	CONSTRAINT pk_user_achivement PRIMARY KEY (id),
	CONSTRAINT unique_user_achievement UNIQUE (user_id, achivement_id),
	CONSTRAINT fk_user_achievement__users FOREIGN KEY (user_id) REFERENCES users(id),
	CONSTRAINT fk_user_achievement__achievement FOREIGN KEY (achivement_id) REFERENCES achivment(id)
);

CREATE TABLE "location" (
	id serial NOT NULL,
	"name" varchar NOT NULL,
	description TEXT NOT NULL,
	CONSTRAINT pk_location PRIMARY KEY (id)
);

CREATE TABLE stand (
	id serial NOT NULL,
	"name" varchar NOT NULL,
	location_id int NOT NULL,
	CONSTRAINT pk_stand PRIMARY KEY (id),
	CONSTRAINT kf_stand__location FOREIGN KEY (location_id) REFERENCES "location"(id)
);

CREATE TABLE showplace (
	id serial NOT NULL,
	"name" varchar NOT NULL,
	description TEXT NOT NULL DEFAULT '',
	location_id int NOT NULL,
	CONSTRAINT pk_showplace PRIMARY KEY (id),
	CONSTRAINT fk_showplace__location FOREIGN KEY (location_id) REFERENCES "location"(id)
);

-- далее можносделать историю посещений
-- но тут вопрос историю чего
-- если локаций - то локации
-- можно отельно записывать историю посещения достопримечательностей

