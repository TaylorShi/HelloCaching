## ASP.NET Core性能优化

### 避免阻塞调用

ASP.NET Core应用应设计为可同时处理许多请求。**异步API允许较小线程池处理数千个并发请求，无需等待阻塞调用**。线程可以处理另一个请求，而不是等待长时间运行的同步任务完成。

ASP.NET Core应用中的一个常见性能问题是**阻塞可以异步进行的调用**。许多同步阻塞调用都会导致线程池饥饿和响应时间降低。

> 禁止行为

- 通过调用`Task.Wait`或`Task<TResult>.Result`.
- 获取常见代码路径中的锁。当构建为并行运行代码时，ASP.NET Core应用的性能最高。
- 调用`Task.Run`并立即等待它。ASP.NET Core已经在普通线程池线程上运行应用代码，因此调用`Task.Run`只会导致不必要的额外线程池计划。即使计划的代码会阻止某个线程，`Task.Run`也不会阻止该线程。

> 建议做法

- 使热代码路径成为异步。
- 如果有异步API可用，则异步调用数据访问、I/O和长时间运行的操作API。请勿用于`Task.Run`使同步API异步。
- 使控制器/RazorPage操作成为异步。为了获益于`async/await`模式，整个调用堆栈都是异步的。

## 相关文章

* [乘风破浪，遇见最佳跨平台跨终端框架.Net Core/.Net生态 - 浅析ASP.NET Core性能设计，使用内存、分布式缓存(Redis)敏捷响应](https://www.cnblogs.com/taylorshi/p/16831521.html)