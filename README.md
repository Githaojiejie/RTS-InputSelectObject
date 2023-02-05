# RTS-InputSelectObject
鼠标拖拽选择场景内的gameobject

Unity 模型描边的几种方法. (Shader、GL、代码生成描绘边）



代码生成描绘边流程:

1.创建so, 选mesh,  点击右上角齿轮(设置)-> gen

2. mytool ->  选择模型, 线框预制体(带有linerender的组件预制体), so文件

原理分析：

原理比较简单，就是获取该模型所有的顶点位置，使用LineRenderer连接两个点，即生成一个边。如果全部连接，就会生成跟GL描边一样的效果。

（题外话：连点成面，连面成网，模型网格是由许多个三角面构成的，而两个三角面即形成一个四边形，至于如何构造三角面形成网格，可以看我之前的文章）

如何剔除掉模型一个面不需要的描边，我们可以使用叉乘，分别获得两个相邻三角面的法线向量，然后对两个法线向量点乘，获得角度，如果两个三角面平行，即它们相交的边可以剔除掉。
参考:[csdn](https://blog.csdn.net/ToToTofu/article/details/106441097)

