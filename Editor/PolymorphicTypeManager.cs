// TODO: Above my current skill, IF T itself is generic, we should allow for
// implementing types that themselves are generic provided they accept the SAME type parameter list
// eg if 'T' were 'IEnumerable<T>', then 'List<T>' is a valid deriving type to be applied, but
// 'Dictionary<TKey, TValue>' would not

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Manages polymorphic types for a specified base type.
/// </summary>
/// <typeparam name="T">The base type for which polymorphic types are managed.</typeparam>
public static class PolymorphicTypeManager<T> where T : class
{
    #region Declarations

    /// <summary>
    /// Represents data for a specific polymorphic type.
    /// </summary>
    public readonly struct TypeData
    {
        private readonly Type m_Type;

        /// <summary>
        /// Gets the name of the polymorphic type.
        /// </summary>
        public readonly string Name => m_Type.Name;

        /// <summary>
        /// Gets an instance of the polymorphic type.
        /// </summary>
        public readonly T Instance => (T) Activator.CreateInstance ( m_Type );

        /// <summary>
        /// Gets the underlying Type object representing the polymorphic type.
        /// </summary>
        public readonly Type Type => m_Type;

        /// <summary>
        /// Initializes a new instance of the TypeData struct.
        /// </summary>
        /// <param name="type">The Type object representing the polymorphic type.</param>
        public TypeData ( Type type )
        {
            m_Type = type;
        }
    }

    #endregion

    #region Fields

    private static readonly List<TypeData> s_Types = new ();
    private static readonly TypeData? s_Default;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of registered polymorphic types.
    /// </summary>
    public static int TypeCount => s_Types.Count;

    /// <summary>
    /// Gets an enumerable collection of TypeData objects representing registered polymorphic types.
    /// </summary>
    public static IEnumerable<TypeData> Types => s_Types;

    public static TypeData? Default => s_Default;

    #endregion

    #region Constructors

    static PolymorphicTypeManager ()
    {
        TypeData? defaultType = null;

        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies ())
        {
            foreach (Type type in asm.GetTypes ())
            {
                if (!type.IsAbstract && !type.IsGenericType && typeof ( T ).IsAssignableFrom ( type ))
                {
                    s_Types.Add ( new TypeData ( type ) );

                    if (type.GetCustomAttribute<DefaultPolymorphicTypeAttribute> () != null && defaultType != null)
                    {
                        defaultType = new TypeData ( type );
                    }
                }
            }

            s_Default = Types.Any () ? defaultType ?? Types.First () : defaultType;
        }
    }

    #endregion
}