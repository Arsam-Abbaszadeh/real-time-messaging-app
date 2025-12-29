namespace realTimeMessagingWebAppInfra.Storage.Utilities;

public static class ObjectStorageServiceUtilities
{
    public static string GenerateObjectKeyForChatFile(Guid userId, Guid chatId, string fileExtension)
    {
        // could add date paths as well
        return $"{chatId}/chats/{userId}/{Guid.NewGuid()}.{fileExtension}";
    }


}
