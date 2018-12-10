using System;

namespace DescentCore.Exceptions {
    //////////////////////////////////////////////////////////////////////
    // Exceptions
    //////////////////////////////////////////////////////////////////////
    public class EquipmentException: Exception { 
        public EquipmentException() { }
        public EquipmentException(string message) : base(message) { }
        public EquipmentException(string message, Exception inner) 
            : base(message, inner) { }
    }

    public class AbillityException: Exception { 
        public AbillityException() { }
        public AbillityException(string message) : base(message) { }
        public AbillityException(string message, Exception inner) 
            : base(message, inner) { }
    }

    public class DieException: Exception { 
        public DieException() { }
        public DieException(string message) : base(message) { }
        public DieException(string message, Exception inner) 
            : base(message, inner) { }
    }
}


