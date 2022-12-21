# One

## 代码规范

- `able`：子项目后缀统一加`able`
- `_Test`: 单元测试以`_Test`为后缀
- `DateTimeOffset`：时间统一使用`DateTimeOffset`
- `Abstractions`：抽象项目，无具体实现

## 项目

- `Infrastructure`: 基础设施
  - `Utility`: 工具集
  - `Module`: 模块化支持（参考[abp](https://abp.io))
  - `Event`: 事件
  - `Domain`: 领域层
  - [`Log`](./log.md)：日志（参考[Serial](https://serilog.net/)）
    - `OneI.Logable`：主项目
    - `OneI.Logable.CodeGenerator`：源代码生成器，生成日志的扩展
    - `OneI.Logable.File`：将日志记录到文件中
  - `Reflectable`：先使用源生成器定位，然后使用Fody补充
- `Example`：案例
- `Service`：服务

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
