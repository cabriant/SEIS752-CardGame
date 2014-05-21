-- Twilio
insert into configuration
values (0, 1, '{"PhoneNumber":"","AccountSid":"","AuthToken":""}');

-- Encryption
insert into configuration
values (1, 1, '{"Password":"","IV":"","Hash":"","KeySize":256,"Salt":"","Iterations":5}');

-- Google Oauth
insert into configuration
values (2, 1, '{"ClientId":"","ClientSecret":"","CallbackUrl":"","Scope":"","SuccessUrl":"","ErrorUrl":""}');