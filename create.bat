:: don't run create_db.sql, assume that normally it already exists
echo starting batch creation
sqlcmd -U taipan-rw -P fakepass -d TaiPan -S DAPHNE-DURON\SQLEXPRESS -i create_schema.sql -i insert_data.sql -i insert_test_hist_data.sql
echo batch creation finished
