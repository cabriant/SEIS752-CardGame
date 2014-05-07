-- create scripts

create database seis752cardgame;

grant delete, insert, select, update on seis752cardgame.* to 'ezadmin'@'localhost';

use seis752cardgame;

-- "User" table
create table user
(
	user_id varchar(50),
	account_type int not null,
	oauth_user_id varchar(50),
	email varchar(255) not null,
	user_pwd varchar(255),
	display_name varchar(30) not null,
	phone_number varchar(35),
	user_type int not null default 0,
	oauth_auth_token varchar(1000),
	oauth_refresh_token varchar(1000),
	account_value int not null default 0
);

alter table user
add primary key (user_id);

alter table user
add constraint user_email_constraint unique (email);

alter table user
add constraint user_disp_name_constraint unique (display_name);

-- "Password reset" table
create table user_pwd_reset
(
	request_id varchar(50) not null,
	user_id varchar(50) not null,
	verification_code varchar(50) not null,
	verification_token varchar(50),
	is_code_valid bit not null default 1,
	is_token_valid bit not null default 0,
	code_sent_to varchar(255) not null,
	sent_date timestamp not null default now()
);

alter table user_pwd_reset
add primary key (request_id);

alter table user_pwd_reset
add constraint fk_user_pwd_reset_user_id foreign key (user_id) references user(user_id);

-- "Poker table" table
create table poker_table
(
	table_id varchar(50),
	table_game_type int not null,
	table_disp_name varchar(50) not null,
	ante int,
	max_raise int,
	max_players int not null,
	table_deck varchar(4000)
);

alter table poker_table
add primary key (table_id);

-- "Player Table" table
create table player_table
(
	user_id varchar(50) not null,
	table_id varchar(50) not null
);

alter table player_table
add primary key (user_id, table_id);

alter table player_table
add constraint fk_player_table_user_id foreign key (user_id) references user(user_id);

alter table player_table
add constraint fk_player_table_table_id foreign key (table_id) references poker_table(table_id);

-- "Game" table
create table game
(
	game_id varchar(50),
	table_id varchar(50) not null,
	game_state int not null,
	house_cards varchar(500),
	table_pot_value int not null,
	current_round_bet int not null,
	req_player_action varchar(1000),
	last_action varchar(255),
	last_raise_value int not null
);

alter table game
add primary key (game_id);

-- "Player game" table
create table player_game
(
	user_id varchar(50),
	game_id varchar(50),
	ante_bet int,
	amt_won_lost int,
	player_hand varchar(500),
	has_anted_bet bit not null default 0,
	player_actions varchar(1000)
);

alter table player_game
add primary key (user_id, game_id);

alter table player_game
add constraint fk_player_game_user_id foreign key (user_id) references user(user_id);

alter table player_game
add constraint fk_player_game_game_id foreign key (game_id) references game(game_id);

-- "Configuration" table
create table configuration
(
	config_type int not null,
	version int not null,
	config varchar(1000)
);

alter table configuration
add primary key (config_type, version);