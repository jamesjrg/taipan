echo starting
sqlcmd -d TaiPan -S (local) -i give_user_permissions.sql
sqlcmd -d TaiPanTest -S (local) -i give_user_permissions.sql
echo finished
