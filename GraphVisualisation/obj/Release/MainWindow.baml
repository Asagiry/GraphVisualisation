   M S B A M L   `   `   `  ���� M  IGraphVisualisation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:GraphVisualisationGraphVisualisation  #    GraphVisualisation.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     2local clr-namespace:GraphVisualisation     -     � Title$  
MainWindow��$	��800��$
��1600��.����  �� CornerRadius$ 5��
���   �$��1q�._��� -    ��Name$ canvas��.+��� 
���   �$��1q� �� Left$	 650��
 �� Top$	 540��$	��250��$��50��$��Center=�$	��Top=�._��� $	 649��$	 550��$	��250��$��50��$��18(�$6��0Введите количество вершин��$��Center=�$��Center=��� -    ��Name$ 
inputNodes��$	 650��$	 595��$	��250��$��50��$��30(�$��Center=�$��Center=�$	��Top=��� -   $	 650��$	 650��$��50��$	��250��$��Добавить граф��$
��Left=�$��Center=�$��18(� � ToolTip _� $��16(�.��TRДобавляет текущий граф в список ваших графов�� 
���   �$��1q�$	 950��$ 95��$	��600��$	��300��._��� $
��Auto=�.��� -    �Name$ 	gridNodes��$	��300�� � ColumnDefinitions �� $	��100 ��� $	��100 ��� 
���   �$��1q�$	 950��$ 40��$	��300��$��50��._��� $��18(�$*��$Введите ребра графа��$��Center=�$��Center=��� 
���   �$��1q�$	��460��$	��855��$ 45��$ 40��._��� $
��Auto=�.��� -   $ 
gridGraphs�� �� $	��205 ��� $	��205 ��� $	��205 ��� $	��205 �	 � RowDefinitions	 �� $	��205 ��� $	��205 ��� $	��205 ��� -   $	 395��$	 650��$��50��$	��250��$-��'Случайное заполнение��$
��Left=�$��Center=�$��18(� _� $��18(�.���� .+��� TRСлучайное заполнение текущего графа ребрами�� :8C указанным количеством вершин�� 
���   �$��1q�$
 1255��$ 40��$	��300��$��50��$
��Left=�$��Center=�._��� $��18(�$��Текущий граф��$��Center=�$��Center=��� -   $ 45��$	 650��$��50��$	��250��$��Center=�$	��Top=�
���   �$��1q�$��Меню действий��$��18(��� 
���   �$��1q�$ 45��$	 595��$	��250��$��50��$
��Left=�$��Center=�._��� $	��250��$��50��$��18(�$*��$Действия с графамии��$��Center=�$��Center=�$��Center=�