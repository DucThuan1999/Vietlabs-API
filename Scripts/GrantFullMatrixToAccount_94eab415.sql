/*
  Cấp FULL ma trận quyền (mọi module × mọi hành động hiện có) cho một account.
  Account: 94eab415-1624-49de-85a6-a80916db3ab2

  Chạy trên SQL Server sau khi đã có bảng account_module_grant + security_module_matrix_action.
  Xóa hết grant cũ của account này rồi gán lại đủ ô (tránh thừa/thiếu).
*/
SET NOCOUNT ON;

DECLARE @AccountId UNIQUEIDENTIFIER = '94eab415-1624-49de-85a6-a80916db3ab2';

IF NOT EXISTS (SELECT 1 FROM dbo.account WHERE account_id = @AccountId)
BEGIN
    RAISERROR(N'Không tìm thấy account_id trong bảng account.', 16, 1);
    RETURN;
END;

DELETE FROM dbo.account_module_grant WHERE account_id = @AccountId;

INSERT INTO dbo.account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), @AccountId, sma.security_module_id, sma.matrix_action_id
FROM dbo.security_module_matrix_action sma;

DECLARE @n INT = @@ROWCOUNT;
PRINT N'Đã cấp ' + CAST(@n AS NVARCHAR(10)) + N' ô quyền cho account ' + CAST(@AccountId AS NVARCHAR(36)) + N'.';
GO
