### **管拱建模流程**

#### 拱建模

 1. 设置拱系
 2. 配置H0,H1等，生成拱对象;
 2. 为拱对象添加关键截面，添加桁片杆件尺寸；
 3. 利用关键截面生成节段对象；

    ```C#
    ArchAxis theArchAxis = new ArchAxis(100, 1.5, 500);
    Arch m1 = new Arch(theArchAxis, 7, 14);
    m1.AddSection(1, 0, Math.PI * 0.5, SectionType.InstallSection);

    ```
 
            




#### 立柱建模


#### 上部结构建模


#### 拱脚建模
