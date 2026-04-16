1. Kiến trúc (Architecture)
Core Systems:
*GridManager
  - Điều khiển toàn bộ logic game
  - Load /Reset/Next Level
  - Xử lý click, tính đường đi và kiểm tra thắng
*ArrowView
  - Hiển thị mũi tên trong game
  - Xử lý animation (move, shake khi bị block)
  - Xoay theo hướng dữ liệu
*ArrowData
  - Lưu trạng thái logic của ô trên grid
  - Gồm hướng (Direction) và trạng thái bị xóa
*LevelData (ScriptableObject)
  - Lưu dữ liệu level (grid size, optimal steps, vị trí arrow)
  - Tách dữ liệu level khỏi code logic
*CubeController
  - Điều khiển xoay cube (visual)
  - Đồng bộ FaceRoot theo cube transform

2. Lời giải (Solution – 3 Level)
Level 1
1. Click (4,1)
2. Click (3,2)
3. Click (2,1)
4. Click (1,3)
5. Click (2,3)
→ Win

Level 2
1. Click (3,4)
2. Click (4,3)
3. Click (2,1)
4. Click (2,3)
5. Click (2,2)
6. Click (3,2)
7. Click (4,2)
→ Win

Level 3
1. Click (4,4)
2. Click (4,3)
3. Click (2,3)
4. Click (3,2)
5. Click (3,3)
6. Click (1,3)
7. Click (1,1)
8. Click (2,1)
9. Click (2,2)
10. Click (5,2)
→ Win

3. Những gì đã làm được
- Hệ thống grid 2D
- Cơ chế click arrow để di chuyển theo hướng
- Kiểm tra va chạm và đường đi hợp lệ
- Win condition + tính sao theo số bước
- Hệ thống level bằng ScriptableObject
- Animation di chuyển bằng DOTween
- Feedback khi bị chặn (shake effect)
- Hệ thống xoay cube (visual)
- Spawn arrow theo level data
- Reset/Next level system

4. Những gì chưa làm được
- Logic chưa hoàn toàn 3d (vẫn dựa trên grid 2D cố định)
- Hướng di chuyển chưa phụ thuộc hoàn toàn vào mặt cube
- Chưa có level editor trong Unity (phải set thủ công)
- Chưa có undo / redo move
- Chưa có hint system
- Chưa có fail state (chỉ có win condition)
