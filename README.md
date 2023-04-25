# One

## 代码规范

- `able`：子项目后缀统一加`able`
- `_Test`: 单元测试以`_Test`为后缀
- `DateTimeOffset`：时间统一使用`DateTimeOffset`
- `Abstractions`：抽象项目，无具体实现
- `CodeGenerator`：代码生成器生成类文件统一命名空间后缀：`CodeGenerated`

## 项目

- `Infrastructure`: 基础设施
  - `Application`: 轻量级应用程序
  - `Utility`: 工具集
  - `Module`: 模块化支持（参考[abp](https://abp.io))
  - [`Log`](./log.md)：日志（参考[Serial](https://serilog.net/)）
    - `OneI.Logable`：主项目
    - `OneI.Logable.CodeGenerator`：源代码生成器，生成日志的扩展
    - `OneI.Logable.File`：将日志记录到文件中
  - `Open`：基于中间件处理TCP连接的服务框架

## 笔记

- `Signal`: 信号，进程间异步通知
- 创建一个泛型对象：Activator.CreateInstance

### Source Code Generator

`OutputItemType="Analyzer" ReferenceOutputAssembly="false"`

- 源生成器类不能继承其他类

## 其他

- git base: `git config --global --add core.quotepath false` 防止中文乱码
- IL指令速查：<https://www.cnblogs.com/flyingbirds123/archive/2011/01/29/1947626.html>
- dotnet源码速查：<https://source.dot.net/>
