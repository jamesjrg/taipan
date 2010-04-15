:: don't run create_db.sql, assume that normally it already exists
echo starting sql data reset
sqlcmd -U taipan-rw -P fakepass -d TaiPan -S DAPHNE-DURON\SQLEXPRESS -i truncate_tables.sql -i insert_data.sql -i insert_test_hist_data.sql
echo sql data reset finished
