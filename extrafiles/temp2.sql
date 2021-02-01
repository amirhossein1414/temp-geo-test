select id,contect from geotable
where ST_Contains(ST_GeomFromText('POLYGON((51.416848734101336 35.69283767592188 
								  ,51.41780127313504 35.69283767592188 
								  ,51.41780127313504 35.69256730486269 
								  ,51.416848734101336 35.69256730486269
								  ,51.416848734101336 35.69283767592188))'),geotable.location
				  --ST_GeomFromText(concat('Point','(',geotable.geodata[0],' ',geotable.geodata[1] , ')'))
				 ) limit 2000;










select ST_GeomFromText('Point(51.398358 35.662543)');

INSERT INTO supermarkets(id,location) VALUES(2,ST_GeomFromText('Point(51.398358 35.662543)'))
INSERT INTO supermarkets(id,location) VALUES('830e7cfb-e377-4e40-9895-a48ff2c128a1',ST_GeomFromText('Point(51.398358 35.662543)'))

delete from supermarkets;

UPDATE geotable SET 
"location" = ST_GeomFromText(concat('Point','(',geotable.geodata[0],' ',geotable.geodata[1] , ')'))


where id in (select id from  geotable where id = 'd113d2e8-7cbd-470e-8723-354bcb742a42'	)	
			 
			 
	 
 select ST_AsText(location) from geotable where id = 'd113d2e8-7cbd-470e-8723-354bcb742a42';
 
 VACUUM (VERBOSE, ANALYZE) geotable;
 
 show data_directory
