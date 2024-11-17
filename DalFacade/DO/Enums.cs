namespace DO;

// Enum definition for user roles
public enum Role
{
    Volunteer,   // Volunteer
    Boss         // Manager
}

// Enum definition for types of distance
public enum Distance
{
    Aerial,      // Aerial distance
    Walking,     // Walking distance
    Driving      // Driving distance
}

// Enum definition for types of calls
public enum CallType
{
    FoodPreparation,  // Food preparation
    FoodTransport,    // Food transport
    InventoryCheck,   // Inventory check
}

// Enum definition for end types of tasks
public enum TypeEnd
{
    Treated,          // Treated
    SelfCancel,       // Self-cancelled
    ManagerCancel,    // Manager-cancelled
    ExpiredCancel     // Expired cancellation
}
