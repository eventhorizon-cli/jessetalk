USE mysql;
UPDATE user 
	SET host = '%' 
WHERE
	user = 'root';
GRANT ALL PRIVILEGES ON *.* TO 'root' @'%' IDENTIFIED BY 'root' WITH GRANT OPTION;
FLUSH PRIVILEGES;