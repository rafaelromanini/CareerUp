namespace CareerUp.Models.Enums
{
    /// <summary>
    /// Papel/função do usuário no sistema.
    /// Valores: 0 = USUARIO (padrão), 1 = GERENTE
    /// </summary>
    public enum PapelUsuario
    {
        /// <summary>
        /// Usuário comum (valor: 0)
        /// </summary>
        USUARIO = 0,

        /// <summary>
        /// Gerente com permissões administrativas (valor: 1)
        /// </summary>
        GERENTE = 1
    }
}
