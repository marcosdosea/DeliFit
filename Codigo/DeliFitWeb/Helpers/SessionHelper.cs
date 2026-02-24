using System.Text.Json;

namespace DeliFitWeb.Helpers
{
    public static class SessionHelper
    {
        private const string ClienteIdKey = "ClienteId";
        private const string RestauranteIdKey = "RestauranteId";
        private const string UserEmailKey = "UserEmail";
        private const string UserRoleKey = "UserRole";

        // Cliente
        public static void SetClienteId(this ISession session, uint clienteId)
        {
            session.SetInt32(ClienteIdKey, (int)clienteId);
        }

        public static uint? GetClienteId(this ISession session)
        {
            var value = session.GetInt32(ClienteIdKey);
            return value.HasValue ? (uint)value.Value : null;
        }

        // Restaurante
        public static void SetRestauranteId(this ISession session, uint restauranteId)
        {
            session.SetInt32(RestauranteIdKey, (int)restauranteId);
        }

        public static uint? GetRestauranteId(this ISession session)
        {
            var value = session.GetInt32(RestauranteIdKey);
            return value.HasValue ? (uint)value.Value : null;
        }

        // Email do usuário
        public static void SetUserEmail(this ISession session, string email)
        {
            session.SetString(UserEmailKey, email);
        }

        public static string GetUserEmail(this ISession session)
        {
            return session.GetString(UserEmailKey);
        }

        // Role do usuário
        public static void SetUserRole(this ISession session, string role)
        {
            session.SetString(UserRoleKey, role);
        }

        public static string GetUserRole(this ISession session)
        {
            return session.GetString(UserRoleKey);
        }

        // Limpar todos os dados da sessão
        public static void ClearUserData(this ISession session)
        {
            session.Remove(ClienteIdKey);
            session.Remove(RestauranteIdKey);
            session.Remove(UserEmailKey);
            session.Remove(UserRoleKey);
        }

        // Métodos auxiliares para armazenar objetos complexos (opcional)
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
