select * from geotable
where ST_Contains(ST_GeomFromText('POLYGON((51.3361930847168
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
				  limit 10;
				  
				  

select count(*)
from geotable where 
geotable.geodata IS NOT NULL and 
ST_Contains(ST_GeomFromText('POLYGON((51.39008677514684 35.69073832733194 ,51.3910393141806 35.69073832733194 ,51.3910393141806 35.69046794915624 ,51.39008677514684 35.69046794915624 ,51.39008677514684 35.69073832733194))'),
			ST_GeomFromText(concat('Point','(',geotable.geodata[0],' ',geotable.geodata[1] , ')')))
			
			and geotable.id = '1';
				  
				  
				  
				  
				  
				  
				  ST_GeomFromText('POLYGON((49.99939521178467 35.00015572680999
								  ,50.00057022297355 35.00015572680999 
								  ,50.00057022297355 34.9998193364313 
								  ,49.99939521178467 34.9998193364313 
								  ,49.99939521178467 35.00015572680999))'