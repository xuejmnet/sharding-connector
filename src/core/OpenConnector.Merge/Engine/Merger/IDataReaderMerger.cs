﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenConnector.CommandParser.Abstractions;
using OpenConnector.CommandParser.Command;
using OpenConnector.CommandParserBinder.Command;
using OpenConnector.CommandParserBinder.MetaData.Schema;
using OpenConnector.StreamDataReaders;

namespace OpenConnector.Merge.Engine.Merger
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/25 10:36:05
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public interface IDataReaderMerger
    {
        IStreamDataReader Merge(List<IStreamDataReader> streamDataReaders, ISqlCommandContext<ISqlCommand> sqlCommandContext, SchemaMetaData schemaMetaData);

    }
}
