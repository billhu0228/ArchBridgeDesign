### **管拱建模流程**

#### 拱建模

**基本步骤**

1. 设置拱系
  ```C#
  ArchAxis theArchAxis = new ArchAxis(418 / f, 1.55, 518);
  Arch m1 = new Arch(theArchAxis, 8.5, 17);
  ```
2. 生成桁架
  ```C#
  m1.GenerateTruss(10, 50, 10, 7.8, 11,2);
  m1.GenerateMiddleDatum(); 生成中插平面
  m1.AddDatum(0, -518 * 0.5, eDatumType.NormalDatum);
  m1.AddDatum(0, 518 * 0.5, eDatumType.NormalDatum);
  m1.AddDatum(0, -253, eDatumType.NormalDatum);
  m1.AddDatum(0, 253, eDatumType.NormalDatum);
  ```

3. 生成上下弦骨架
  ```C#
  m1.GenerateSkeleton();
  ```

4. 配置截面
  ```C#
  var s1 = new TubeSection(1.4, 0.035);
  m1.AssignProperty(eMemberType.UpperCoord, s1);
  m1.AssignProperty(eMemberType.LowerCoord, s1);
  m1.AssignProperty(eMemberType.VerticalWeb, new TubeSection(0.9, 0.024));
  m1.AssignProperty(eMemberType.ColumnWeb, new TubeSection(0.9, 0.024));
  m1.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.9, 0.024));
  ```

5. 生成斜杆基准面
  ```C#
  m1.GenerateDiagonalDatum(0.060);
  ```

6. 生成模型
  ```C#
  m1.GenerateModel();
  ```
            




#### 立柱建模


#### 上部结构建模


#### 拱脚建模
