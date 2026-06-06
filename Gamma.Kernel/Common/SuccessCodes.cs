namespace Gamma.Kernel.Common;

public static class SuccessCodes
{
    // =====================================================
    // Generic
    // =====================================================

    public const string Success = "SUCCESS";

    // =====================================================
    // CRUD
    // =====================================================

    public const string EntityCreated = "ENTITY_CREATED";
    public const string EntityUpdated = "ENTITY_UPDATED";
    public const string EntityDeleted = "ENTITY_DELETED";

    // =====================================================
    // Batch
    // =====================================================

    public const string BatchUpdated = "BATCH_UPDATED";
    public const string BatchDeleted = "BATCH_DELETED";
    public const string BatchActivated = "BATCH_ACTIVATED";
    public const string BatchDeactivated = "BATCH_DEACTIVATED";

    // =====================================================
    // Status Changes
    // =====================================================

    public const string EntityActivated = "ENTITY_ACTIVATED";
    public const string EntityDeactivated = "ENTITY_DEACTIVATED";

    // =====================================================
    // Workflow
    // =====================================================

    public const string Approved = "APPROVED";
    public const string Rejected = "REJECTED";
    public const string Posted = "POSTED";
    public const string Unposted = "UNPOSTED";
    public const string Closed = "CLOSED";
    public const string Reopened = "REOPENED";

    // =====================================================
    // Import / Export
    // =====================================================

    public const string Imported = "IMPORTED";
    public const string Exported = "EXPORTED";
}