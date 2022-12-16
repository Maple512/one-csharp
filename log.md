# 日志模块

## 模版

- `{Property[,alignment][:format]['indent]}`

Property: 字母，_，数字

align：整数，>=0 右对齐，<0 左对齐
```txt
右对齐
***message
*****error
左对齐
message*
error***
```

indent：缩进，>= 0的整数
