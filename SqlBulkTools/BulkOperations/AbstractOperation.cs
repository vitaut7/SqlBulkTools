﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using SqlBulkTools.Enumeration;

namespace SqlBulkTools
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractOperation<T>
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        // ReSharper disable InconsistentNaming
        protected ColumnDirectionType _outputIdentity;
        protected string _identityColumn;
        protected Dictionary<int, T> _outputIdentityDic;
        protected bool _disableAllIndexes;
        protected int _sqlTimeout;
        protected HashSet<string> _columns;
        protected string _schema;
        protected string _tableName;
        protected Dictionary<string, string> _customColumnMappings;
        protected IEnumerable<T> _list;
        protected List<string> _matchTargetOn;
        protected List<PredicateCondition> _updatePredicates;
        protected List<PredicateCondition> _deletePredicates;
        protected List<SqlParameter> _parameters;
        protected Dictionary<string, string> _collationColumnDic;
        protected int _conditionSortOrder;
        protected BulkCopySettings _bulkCopySettings;
        protected string _tableHint;
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <param name="columns"></param>
        /// <param name="customColumnMappings"></param>
        /// <param name="bulkCopySettings"></param>
        protected AbstractOperation(IEnumerable<T> list, string tableName, string schema, HashSet<string> columns,
            Dictionary<string, string> customColumnMappings, BulkCopySettings bulkCopySettings)
        {
            _list = list;
            _tableName = tableName;
            _schema = schema;
            _columns = columns;
            _disableAllIndexes = false;
            _customColumnMappings = customColumnMappings;
            _identityColumn = null;
            _collationColumnDic = new Dictionary<string, string>();
            _outputIdentityDic = new Dictionary<int, T>();
            _outputIdentity = ColumnDirectionType.Input;
            _matchTargetOn = new List<string>();
            _bulkCopySettings = bulkCopySettings;
            _tableHint = "HOLDLOCK";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <exception cref="SqlBulkToolsException"></exception>

        protected void SetIdentity(Expression<Func<T, object>> columnName)
        {
            var propertyName = BulkOperationsHelper.GetPropertyName(columnName);

            if (propertyName == null)
                throw new SqlBulkToolsException("SetIdentityColumn column name can't be null");

            if (_identityColumn == null)
                _identityColumn = propertyName;

            else
            {
                throw new SqlBulkToolsException("Can't have more than one identity column");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterToCheck"></param>
        /// <typeparam name="TParameter"></typeparam>
        /// <returns></returns>
        public static TParameter GetParameterValue<TParameter>(Expression<Func<TParameter>> parameterToCheck)
        {
            TParameter parameterValue = (TParameter)parameterToCheck.Compile().Invoke();

            return parameterValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="outputIdentity"></param>
        protected void SetIdentity(Expression<Func<T, object>> columnName, ColumnDirectionType outputIdentity)
        {
            _outputIdentity = outputIdentity;
            SetIdentity(columnName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="collation"></param>
        /// <returns></returns>
        protected void SetCollation(Expression<Func<T, object>> columnName, string collation)
        {
            var propertyName = BulkOperationsHelper.GetPropertyName(columnName);

            if (propertyName == null)
                throw new SqlBulkToolsException("Collation can't be null");

            _collationColumnDic.Add(propertyName, collation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="SqlBulkToolsException"></exception>
        protected void MatchTargetCheck()
        {
            if (_matchTargetOn.Count == 0)
            {
                throw new SqlBulkToolsException("MatchTargetOn list is empty when it's required for this operation. " +
                                                    "This is usually the primary key of your table but can also be more than one " +
                                                    "column depending on your business rules.");
            }
        }
    }
}
