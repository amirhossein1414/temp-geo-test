select * from geotable where ST_Contains(ST_GeomFromText('POLYGON((51.3361930847168
              35.705959231097545,
														 
			  51.30821228027344 
              35.622698214535184,
														 
			  51.44073486328125 
              35.62381451392674,
														 
			  51.433353424072266 
              35.70777131894265,
														 
			  51.3361930847168 
              35.705959231097545))'),
			  ST_GeomFromText(concat('Point','(',geotable.geodata[0],' ',geotable.geodata[1] , ')')))
			  and id ='6e4e4ccd-9101-4579-badf-29c436714a01'
			   limit 1000;
			   
SELECT ST_Distance(ST_GeomFromText(concat('Point','(',n1.geodata[0],' ',n1.geodata[1] , ')'))
				   , ST_GeomFromText(concat('Point(51.398358 35.662543)'))) as d,id,geodata
from (select * from geotable limit 1000) n1
ORDER BY d
limit 10;
			   
select * from geotable where id ='6e4e4ccd-9101-4579-badf-29c436714a01';
							
select ST_GeomFromText(concat('Point','(',geotable.geodata[0],' ',geotable.geodata[1] , ')')) from geotable limit 10;							
select concat('(',geotable.geodata[0],' ',geotable.geodata[1] , ')') from geotable limit 10;

										
										ST_GeomFromText('POINT(51.360676288604736 
            35.69803086246004)')

SELECT ST_GeomFromText('POLYGON((51.3361930847168 35.705959231097545,
			  51.30821228027344 35.622698214535184,
			  51.44073486328125 35.62381451392674,
			  51.433353424072266 35.70777131894265,
			  51.433353424072266 35.70777131894265))');
										 
select ST_GeomFromText('POINT(51.360676288604736 35.69803086246004)');