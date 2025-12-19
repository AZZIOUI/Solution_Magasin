namespace Solution_Magasin.Constants
{
    /// <summary>
    /// Constantes pour les rôles de l'application
    /// </summary>
    public static class RoleConstants
    {
        // Rôles Client
        public const string Client = "Client";

        // Rôles Employé
        public const string Administrateur = "Administrateur";
        public const string ResponsableAchat = "ResponsableAchat";
        public const string Magasinier = "Magasinier";

        // Politiques
        public const string ClientPolicy = "ClientOnly";
        public const string EmployePolicy = "EmployeOnly";
        public const string AdminPolicy = "AdminOnly";
        public const string ResponsableAchatPolicy = "ResponsableAchatOnly";
        public const string MagasinierPolicy = "MagasinierOnly";

        /// <summary>
        /// Retourne tous les rôles employé
        /// </summary>
        public static string[] GetEmployeeRoles()
        {
            return new[] { Administrateur, ResponsableAchat, Magasinier };
        }

        /// <summary>
        /// Retourne tous les rôles de l'application
        /// </summary>
        public static string[] GetAllRoles()
        {
            return new[] { Client, Administrateur, ResponsableAchat, Magasinier };
        }
    }
}
