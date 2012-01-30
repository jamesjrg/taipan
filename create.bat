:: don't run create_db.sql, assume that normally it already exists
echo starting batch creation
sqlcmd -U taipan-rw -P fakepass -d TaiPan -S (local) -i create_schema.sql -i insert_data.sql
sqlcmd -U taipan-rw -P fakepass -d TaiPan -S (local) -i create_schema.sql -i insert_data.sql
echo batch creation finished
