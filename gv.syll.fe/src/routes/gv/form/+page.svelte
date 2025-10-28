<script>
	// Dynamic table functions
	
</script>

<div class="container">
	<!-- Alert -->
	<div class="alert alert-info">
		<span>ℹ️</span>
		<span
			>Các trường có nền màu xanh là dữ liệu đã được đồng bộ từ hệ thống TCCB. Bạn có thể chỉnh sửa
			nếu cần thiết.</span
		>
	</div>

	<!-- Form Header -->
	<div class="form-header">
		<div class="form-title">
			<h1>SƠ YẾU LÝ LỊCH</h1>
			<p>VIÊN CHỨC</p>
		</div>

		<div class="form-meta">
			<div class="form-group">
				<label>Cơ quan quản lý:</label>
				<input type="text" value="TRƯỜNG ĐẠI HỌC XÂY DỰNG HÀ NỘI" readonly class="synced-field" />
			</div>
			<div class="form-group">
				<label>Đơn vị sử dụng:</label>
				<input type="text" placeholder="Nhập đơn vị" class="synced-field" />
			</div>
			<div class="form-row-2" style="grid-column: span 1;">
				<div class="form-group">
					<label>Số hiệu:</label>
					<input type="text" placeholder="Số hiệu" class="synced-field" />
				</div>
				<div class="form-group">
					<label>Mã định danh:</label>
					<input type="text" placeholder="Mã định danh" class="synced-field" />
				</div>
			</div>
		</div>
	</div>

	<!-- Section 1: Thông tin cá nhân cơ bản -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>👤</span>
				<span>THÔNG TIN CÁ NHÂN CƠ BẢN</span>
			</h2>
		</div>

		<div class="photo-section">
			<div class="photo-upload">
				<div style="text-align: center;">
					<div style="font-size: 32px;">📷</div>
					<div>Ảnh màu 4x6 cm</div>
				</div>
			</div>
			<input type="file" id="photoInput" accept="image/*" style="display: none;" />

			<div class="basic-info">
				<div class="form-row-2">
					<div class="form-group">
						<label>1) Họ và tên khai sinh (viết IN HOA) <span class="required">*</span></label>
						<input
							type="text"
							placeholder="NGUYỄN VĂN AN"
							class="synced-field"
							style="text-transform: uppercase;"
							required
						/>
					</div>
					<div class="form-group">
						<label>Giới tính <span class="required">*</span></label>
						<select class="synced-field" required>
							<option value="">Chọn giới tính</option>
							<option value="nam">Nam</option>
							<option value="nu">Nữ</option>
						</select>
					</div>
				</div>

				<div class="form-group">
					<label>2) Các tên gọi khác</label>
					<input type="text" placeholder="Nếu có" />
				</div>

				<div class="form-row-3">
					<div class="form-group">
						<label>3) Sinh ngày <span class="required">*</span></label>
						<input type="date" class="synced-field" required />
					</div>
					<div class="form-group" style="grid-column: span 2;">
						<label>4) Nơi sinh</label>
						<input type="text" placeholder="Tỉnh/Thành phố" />
					</div>
				</div>
			</div>
		</div>

		<div class="form-section">
			<div class="form-row-2">
				<div class="form-group">
					<label>5) Quê quán</label>
					<input type="text" placeholder="Xã/Phường, Huyện/Quận, Tỉnh/TP" />
				</div>
				<div class="form-row-2">
					<div class="form-group">
						<label>6) Dân tộc</label>
						<select>
							<option value="">Chọn dân tộc</option>
							<option value="kinh">Kinh</option>
							<option value="tay">Tày</option>
							<option value="thai">Thái</option>
							<option value="muong">Mường</option>
							<option value="hoa">Hoa</option>
							<option value="khmer">Khmer</option>
							<option value="nung">Nùng</option>
							<option value="hmong">H'Mông</option>
						</select>
					</div>
					<div class="form-group">
						<label>7) Tôn giáo</label>
						<select>
							<option value="">Không</option>
							<option value="phat-giao">Phật giáo</option>
							<option value="cong-giao">Công giáo</option>
							<option value="tin-lanh">Tin lành</option>
							<option value="cao-dai">Cao đài</option>
							<option value="hoa-hao">Phật giáo Hòa Hảo</option>
						</select>
					</div>
				</div>
			</div>

			<div class="form-row-4">
				<div class="form-group">
					<label>8) Số CCCD/CMND <span class="required">*</span></label>
					<input type="text" class="synced-field" required />
				</div>
				<div class="form-group">
					<label>Ngày cấp</label>
					<input type="date" />
				</div>
				<div class="form-group">
					<label>Nơi cấp</label>
					<input type="text" placeholder="Nơi cấp" />
				</div>
				<div class="form-group">
					<label>SĐT liên hệ <span class="required">*</span></label>
					<input type="tel" class="synced-field" required />
				</div>
			</div>

			<div class="form-row-2">
				<div class="form-group">
					<label>9) Sổ BHXH</label>
					<input type="text" class="synced-field" />
				</div>
				<div class="form-group">
					<label>Số thẻ BHYT</label>
					<input type="text" class="synced-field" />
				</div>
			</div>

			<div class="form-group">
				<label>10) Nơi ở hiện nay</label>
				<input type="text" placeholder="Địa chỉ hiện tại" />
			</div>

			<div class="form-row-2">
				<div class="form-group">
					<label>11) Thành phần gia đình xuất thân</label>
					<select>
						<option value="">Chọn thành phần</option>
						<option value="cong-nhan">Công nhân</option>
						<option value="nong-dan">Nông dân</option>
						<option value="can-bo">Cán bộ, công chức</option>
						<option value="quan-nhan">Quân nhân</option>
						<option value="tri-thuc">Trí thức</option>
						<option value="tieu-thuong">Tiểu thương</option>
					</select>
				</div>
				<div class="form-group">
					<label>12) Nghề nghiệp trước khi tuyển dụng</label>
					<input type="text" placeholder="Nghề nghiệp" />
				</div>
			</div>
		</div>
	</div>

	<!-- Section 2: Thông tin tuyển dụng và Đảng -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>🏢</span>
				<span>THÔNG TIN TUYỂN DỤNG VÀ TỔ CHỨC</span>
			</h2>
		</div>

		<div class="form-section">
			<div class="form-row-2">
				<div class="form-group">
					<label>13) Ngày tuyển dụng lần đầu</label>
					<input type="date" class="synced-field" />
				</div>
				<div class="form-group">
					<label>Cơ quan, tổ chức, đơn vị tuyển dụng</label>
					<input type="text" placeholder="Tên cơ quan" />
				</div>
			</div>

			<div class="form-group">
				<label>14) Ngày vào cơ quan hiện đang công tác</label>
				<input type="date" class="synced-field" />
			</div>

			<div class="form-row-2">
				<div class="form-group">
					<label>15) Ngày vào Đảng Cộng sản Việt Nam</label>
					<input type="date" />
				</div>
				<div class="form-group">
					<label>Ngày chính thức</label>
					<input type="date" />
				</div>
			</div>

			<div class="form-group">
				<label>16) Ngày tham gia tổ chức chính trị-xã hội đầu tiên</label>
				<input type="text" placeholder="Ngày vào Đoàn TNCS, Công đoàn, Hội..." />
			</div>

			<div class="form-row-3">
				<div class="form-group">
					<label>17) Ngày nhập ngũ</label>
					<input type="date" />
				</div>
				<div class="form-group">
					<label>Ngày xuất ngũ</label>
					<input type="date" />
				</div>
				<div class="form-group">
					<label>Quân hàm cao nhất</label>
					<input type="text" placeholder="Quân hàm" />
				</div>
			</div>

			<div class="form-group">
				<label>18) Đối tượng chính sách</label>
				<input type="text" placeholder="Con liệt sĩ, thương binh, bệnh binh..." />
			</div>
		</div>
	</div>

	<!-- Section 3: Trình độ và chức vụ -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>🎓</span>
				<span>TRÌNH ĐỘ VÀ CHỨC VỤ</span>
			</h2>
		</div>

		<div class="form-section">
			<div class="form-row-2">
				<div class="form-group">
					<label>19) Trình độ giáo dục phổ thông</label>
					<input type="text" placeholder="12/12" class="synced-field" />
				</div>
				<div class="form-group">
					<label>20) Trình độ chuyên môn cao nhất</label>
					<select class="synced-field">
						<option value="">Chọn trình độ</option>
						<option value="trung-cap">Trung cấp</option>
						<option value="cao-dang">Cao đẳng</option>
						<option value="dai-hoc">Đại học</option>
						<option value="thac-si">Thạc sĩ</option>
						<option value="tien-si">Tiến sĩ</option>
						<option value="tien-si-khoa-hoc">Tiến sĩ khoa học</option>
					</select>
				</div>
			</div>

			<div class="form-row-2">
				<div class="form-group">
					<label>21) Học hàm</label>
					<select class="synced-field">
						<option value="">Không</option>
						<option value="giang-vien">Giảng viên</option>
						<option value="giang-vien-chinh">Giảng viên chính</option>
						<option value="pho-giao-su">Phó Giáo sư</option>
						<option value="giao-su">Giáo sư</option>
					</select>
				</div>
				<div class="form-group">
					<label>22) Danh hiệu nhà nước phong tặng</label>
					<input type="text" placeholder="Nhà giáo ưu tú, Nhà giáo nhân dân..." />
				</div>
			</div>

			<div class="form-row-3">
				<div class="form-group">
					<label>23) Chức vụ hiện tại</label>
					<input type="text" class="synced-field" placeholder="Chức vụ" />
				</div>
				<div class="form-group">
					<label>Ngày bổ nhiệm/phê chuẩn</label>
					<input type="date" class="synced-field" />
				</div>
				<div class="form-group">
					<label>Ngày bổ nhiệm lại/phê chuẩn nhiệm kỳ tiếp</label>
					<input type="date" />
				</div>
			</div>

			<div class="form-row-2">
				<div class="form-group">
					<label>24) Được quy hoạch chức danh</label>
					<input type="text" placeholder="Chức danh được quy hoạch" />
				</div>
				<div class="form-group">
					<label>25) Chức vụ kiêm nhiệm</label>
					<input type="text" placeholder="Chức vụ kiêm nhiệm" />
				</div>
			</div>

			<div class="form-row-2">
				<div class="form-group">
					<label>26) Chức vụ Đảng hiện tại</label>
					<input type="text" placeholder="Bí thư, Phó bí thư, Ủy viên..." />
				</div>
				<div class="form-group">
					<label>27) Chức vụ Đảng kiêm nhiệm</label>
					<input type="text" placeholder="Chức vụ Đảng kiêm nhiệm" />
				</div>
			</div>

			<div class="form-group">
				<label>28) Công việc chính được giao</label>
				<textarea placeholder="Mô tả công việc chính"></textarea>
			</div>

			<div class="form-row-2">
				<div class="form-group">
					<label>29) Sở trường công tác</label>
					<input type="text" placeholder="Sở trường" />
				</div>
				<div class="form-group">
					<label>Công việc làm lâu nhất</label>
					<input type="text" placeholder="Công việc làm lâu nhất" />
				</div>
			</div>
		</div>
	</div>

	<!-- Section 4: Tiền lương -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>💰</span>
				<span>THÔNG TIN LƯƠNG VÀ PHỤ CẤP</span>
			</h2>
		</div>

		<div class="form-section">
			<h4 style="margin-bottom: 15px; color: #666;">30) Tiền lương</h4>

			<div style="padding: 15px; background: #f9f9f9; border-radius: 8px; margin-bottom: 20px;">
				<h5 style="margin-bottom: 15px;">30.1) Ngạch/chức danh nghề nghiệp</h5>
				<div class="form-row-2">
					<div class="form-group">
						<label>Ngạch/chức danh nghề nghiệp</label>
						<input type="text" class="synced-field" />
					</div>
					<div class="form-group">
						<label>Mã số</label>
						<input type="text" class="synced-field" />
					</div>
				</div>

				<div class="form-group">
					<label>Ngày bổ nhiệm ngạch/chức danh nghề nghiệp</label>
					<input type="date" class="synced-field" />
				</div>

				<div class="form-row-4">
					<div class="form-group">
						<label>Bậc lương</label>
						<input type="text" class="synced-field" />
					</div>
					<div class="form-group">
						<label>Hệ số</label>
						<input type="text" class="synced-field" />
					</div>
					<div class="form-group">
						<label>Ngày hưởng</label>
						<input type="date" class="synced-field" />
					</div>
					<div class="form-group">
						<label>Phần trăm hưởng (%)</label>
						<input type="number" class="synced-field" value="100" />
					</div>
				</div>

				<div class="form-row-2">
					<div class="form-group">
						<label>Phụ cấp thâm niên vượt khung (%)</label>
						<input type="number" class="synced-field" />
					</div>
					<div class="form-group">
						<label>Ngày hưởng PCTNVK</label>
						<input type="date" class="synced-field" />
					</div>
				</div>
			</div>

			<div style="padding: 15px; background: #f9f9f9; border-radius: 8px; margin-bottom: 20px;">
				<h5 style="margin-bottom: 15px;">30.2) Phụ cấp</h5>
				<div class="form-row-3">
					<div class="form-group">
						<label>Phụ cấp chức vụ</label>
						<input type="text" class="synced-field" />
					</div>
					<div class="form-group">
						<label>Phụ cấp kiêm nhiệm</label>
						<input type="text" />
					</div>
					<div class="form-group">
						<label>Phụ cấp khác</label>
						<input type="text" />
					</div>
				</div>
			</div>

			<div style="padding: 15px; background: #f9f9f9; border-radius: 8px;">
				<h5 style="margin-bottom: 15px;">30.3) Vị trí việc làm</h5>
				<div class="form-row-2">
					<div class="form-group">
						<label>Vị trí việc làm</label>
						<input type="text" class="synced-field" />
					</div>
					<div class="form-group">
						<label>Mã số</label>
						<input type="text" class="synced-field" />
					</div>
				</div>

				<div class="form-row-3">
					<div class="form-group">
						<label>Bậc lương</label>
						<input type="text" />
					</div>
					<div class="form-group">
						<label>Lương theo mức tiền (VNĐ)</label>
						<input type="number" />
					</div>
					<div class="form-group">
						<label>Ngày hưởng</label>
						<input type="date" />
					</div>
				</div>
			</div>
		</div>
	</div>

	<!-- Section 5: Sức khỏe -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>🏥</span>
				<span>TÌNH TRẠNG SỨC KHỎE</span>
			</h2>
		</div>

		<div class="form-section">
			<div class="form-row-4">
				<div class="form-group">
					<label>31) Tình trạng sức khỏe</label>
					<select>
						<option value="">Chọn tình trạng</option>
						<option value="tot">Tốt</option>
						<option value="kha">Khá</option>
						<option value="trung-binh">Trung bình</option>
						<option value="yeu">Yếu</option>
					</select>
				</div>
				<div class="form-group">
					<label>Chiều cao (cm)</label>
					<input type="number" placeholder="170" />
				</div>
				<div class="form-group">
					<label>Cân nặng (kg)</label>
					<input type="number" placeholder="65" />
				</div>
				<div class="form-group">
					<label>Nhóm máu</label>
					<select>
						<option value="">Chọn nhóm máu</option>
						<option value="A">A</option>
						<option value="B">B</option>
						<option value="O">O</option>
						<option value="AB">AB</option>
					</select>
				</div>
			</div>
		</div>
	</div>

	<!-- Section 6: Quá trình đào tạo, bồi dưỡng -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>📚</span>
				<span>32) QUÁ TRÌNH ĐÀO TẠO, BỒI DƯỠNG</span>
			</h2>
		</div>

		<div class="form-section">
			<!-- 32.1 Chuyên môn -->
			<h4 style="margin-bottom: 15px; color: #666;">32.1) Chuyên môn (từ trung cấp trở lên)</h4>
			<div class="dynamic-table">
				<table id="trainingTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Tên cơ sở đào tạo</th>
							<th rowspan="2">Chuyên ngành đào tạo</th>
							<th rowspan="2">Hình thức đào tạo</th>
							<th rowspan="2">Văn bằng, trình độ</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>

			<!-- 32.2 Lý luận chính trị -->
			<h4 style="margin: 30px 0 15px; color: #666;">32.2) Lý luận chính trị</h4>
			<div class="dynamic-table">
				<table id="politicalTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Tên cơ sở đào tạo</th>
							<th rowspan="2">Hình thức đào tạo</th>
							<th rowspan="2">Văn bằng được cấp</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>

			<!-- 32.3 Bồi dưỡng quản lý nhà nước -->
			<h4 style="margin: 30px 0 15px; color: #666;">
				32.3) Bồi dưỡng quản lý nhà nước/chức danh nghề nghiệp
			</h4>
			<div class="dynamic-table">
				<table id="managementTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Tên cơ sở đào tạo</th>
							<th rowspan="2">Chứng chỉ được cấp</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>

			<!-- 32.4 An ninh quốc phòng -->
			<h4 style="margin: 30px 0 15px; color: #666;">
				32.4) Bồi dưỡng kiến thức an ninh, quốc phòng
			</h4>
			<div class="dynamic-table">
				<table id="defenseTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Tên cơ sở đào tạo</th>
							<th rowspan="2">Chứng chỉ được cấp</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>

			<!-- 32.5 Tin học -->
			<h4 style="margin: 30px 0 15px; color: #666;">32.5) Tin học</h4>
			<div class="dynamic-table">
				<table id="itTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Tên cơ sở đào tạo</th>
							<th rowspan="2">Chứng chỉ được cấp</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>

			<!-- 32.6 Ngoại ngữ -->
			<h4 style="margin: 30px 0 15px; color: #666;">32.6) Ngoại ngữ/tiếng dân tộc</h4>
			<div class="dynamic-table">
				<table id="languageTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Tên cơ sở đào tạo</th>
							<th rowspan="2">Tên ngoại ngữ/tiếng dân tộc</th>
							<th rowspan="2">Chứng chỉ được cấp</th>
							<th rowspan="2">Điểm số</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>
		</div>
	</div>

	<!-- Section 7: Quá trình công tác -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>💼</span>
				<span>33) TÓM TẮT QUÁ TRÌNH CÔNG TÁC</span>
			</h2>
		</div>

		<div class="form-section">
			<div class="dynamic-table">
				<table id="workHistoryTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Đơn vị công tác<br />(đảng, chính quyền, đoàn thể, tổ chức xã hội)</th
							>
							<th rowspan="2">Chức danh/chức vụ</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>
		</div>
	</div>

	<!-- Section 8: Đặc điểm lịch sử bản thân -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>📜</span>
				<span>34) ĐẶC ĐIỂM LỊCH SỬ BẢN THÂN</span>
			</h2>
		</div>

		<div class="form-section">
			<div class="form-group">
				<label
					>34.1) Khai rõ: bị bắt, bị tù (từ ngày tháng năm nào đến ngày tháng năm nào, ở đâu?)</label
				>
				<textarea placeholder="Khai báo chi tiết nếu có"></textarea>
			</div>

			<h4 style="margin: 20px 0 15px; color: #666;">34.2) Bản thân có làm việc cho chế độ cũ</h4>
			<div class="dynamic-table">
				<table id="oldRegimeTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Chức danh, chức vụ, đơn vị, địa điểm đã làm việc</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>

			<h4 style="margin: 30px 0 15px; color: #666;">
				34.3) Tham gia hoặc có quan hệ với các tổ chức chính trị, kinh tế, xã hội ở nước ngoài
			</h4>
			<div class="dynamic-table">
				<table id="foreignOrgTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Tên tổ chức, địa chỉ trụ sở, công việc đã làm</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>
		</div>
	</div>

	<!-- Section 9: Khen thưởng, kỷ luật -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>🏆</span>
				<span>35) KHEN THƯỞNG, KỶ LUẬT</span>
			</h2>
		</div>

		<div class="form-section">
			<h4 style="margin-bottom: 15px; color: #666;">35.1) Thành tích thi đua, khen thưởng</h4>
			<div class="dynamic-table">
				<table id="rewardTable">
					<thead>
						<tr>
							<th>Năm</th>
							<th>Xếp loại chuyên môn</th>
							<th>Xếp loại thi đua</th>
							<th>Hình thức khen thưởng</th>
							<th>Thao tác</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="number" placeholder="2024" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>

			<h4 style="margin: 30px 0 15px; color: #666;">35.2) Kỷ luật Đảng/hành chính</h4>
			<div class="dynamic-table">
				<table id="disciplineTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Hình thức</th>
							<th rowspan="2">Hành vi vi phạm chính</th>
							<th rowspan="2">Cơ quan quyết định</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>
		</div>
	</div>

	<!-- Section 10: Quan hệ gia đình -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>👨‍👩‍👧‍👦</span>
				<span>36) QUAN HỆ GIA ĐÌNH</span>
			</h2>
		</div>

		<div class="form-section">
			<h4 style="margin-bottom: 15px; color: #666;">
				36.1) Về bản thân: Cha, Mẹ, Vợ (hoặc chồng), các con, anh chị em ruột
			</h4>
			<div class="dynamic-table">
				<table id="familyTable">
					<thead>
						<tr>
							<th>Mối quan hệ</th>
							<th>Họ và tên</th>
							<th>Năm sinh</th>
							<th>Quê quán, nghề nghiệp, chức danh, chức vụ, đơn vị công tác, học tập, nơi ở</th>
							<th>Thao tác</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td>
								<select>
									<option value="">Chọn quan hệ</option>
									<option value="cha">Cha</option>
									<option value="me">Mẹ</option>
									<option value="vo">Vợ</option>
									<option value="chong">Chồng</option>
									<option value="con">Con</option>
									<option value="anh">Anh ruột</option>
									<option value="chi">Chị ruột</option>
									<option value="em">Em ruột</option>
								</select>
							</td>
							<td><input type="text" /></td>
							<td><input type="number" placeholder="1990" /></td>
							<td><textarea style="min-height: 40px;"></textarea></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm người thân</button>

			<h4 style="margin: 30px 0 15px; color: #666;">
				36.2) Cha, Mẹ, anh chị em ruột (bên vợ hoặc chồng)
			</h4>
			<div class="dynamic-table">
				<table id="inLawTable">
					<thead>
						<tr>
							<th>Mối quan hệ</th>
							<th>Họ và tên</th>
							<th>Năm sinh</th>
							<th>Quê quán, nghề nghiệp, chức danh, chức vụ, đơn vị công tác, học tập, nơi ở</th>
							<th>Thao tác</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td>
								<select>
									<option value="">Chọn quan hệ</option>
									<option value="cha-vo">Cha vợ/chồng</option>
									<option value="me-vo">Mẹ vợ/chồng</option>
									<option value="anh-vo">Anh vợ/chồng</option>
									<option value="chi-vo">Chị vợ/chồng</option>
									<option value="em-vo">Em vợ/chồng</option>
								</select>
							</td>
							<td><input type="text" /></td>
							<td><input type="number" placeholder="1990" /></td>
							<td><textarea style="min-height: 40px;"></textarea></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm người thân</button>
		</div>
	</div>

	<!-- Section 11: Hoàn cảnh kinh tế gia đình -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>💵</span>
				<span>37) HOÀN CẢNH KINH TẾ GIA ĐÌNH</span>
			</h2>
		</div>

		<div class="form-section">
			<h4 style="margin-bottom: 15px; color: #666;">37.1) Quá trình lương của bản thân</h4>
			<div class="dynamic-table">
				<table id="salaryHistoryTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Mã số</th>
							<th rowspan="2">Bậc lương</th>
							<th rowspan="2">Hệ số lương</th>
							<th rowspan="2">Tiền lương theo vị trí việc làm</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="number" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>

			<h4 style="margin: 30px 0 15px; color: #666;">37.2) Các loại phụ cấp khác</h4>
			<div class="dynamic-table">
				<table id="allowanceTable">
					<thead>
						<tr>
							<th colspan="2">Tháng/năm</th>
							<th rowspan="2">Loại phụ cấp</th>
							<th rowspan="2">Phần trăm hưởng</th>
							<th rowspan="2">Hệ số</th>
							<th rowspan="2">Hình thức hưởng</th>
							<th rowspan="2">Giá trị (đồng)</th>
							<th rowspan="2">Thao tác</th>
						</tr>
						<tr>
							<th>Từ</th>
							<th>Đến</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td><input type="month" /></td>
							<td><input type="month" /></td>
							<td><input type="text" /></td>
							<td><input type="number" /></td>
							<td><input type="text" /></td>
							<td><input type="text" /></td>
							<td><input type="number" /></td>
							<td><button class="remove-btn">Xóa</button></td>
						</tr>
					</tbody>
				</table>
			</div>
			<button class="add-row-btn">+ Thêm dòng</button>

			<h4 style="margin: 30px 0 15px; color: #666;">
				37.3) Nguồn thu nhập chính của gia đình hàng năm
			</h4>
			<div class="form-row-2">
				<div class="form-group">
					<label>Tiền lương</label>
					<input type="text" placeholder="VNĐ/năm" />
				</div>
				<div class="form-group">
					<label>Các nguồn khác</label>
					<input type="text" placeholder="Mô tả nguồn thu khác" />
				</div>
			</div>

			<h5 style="margin: 20px 0 15px; color: #666;">Nhà ở</h5>
			<div class="form-row-2">
				<div class="form-group">
					<label>Được cấp, được thuê (loại nhà)</label>
					<input type="text" placeholder="Loại nhà" />
				</div>
				<div class="form-group">
					<label>Tổng diện tích sử dụng (m²)</label>
					<input type="number" placeholder="m²" />
				</div>
			</div>
			<div class="form-group">
				<label>Giấy chứng nhận quyền sở hữu</label>
				<input type="text" placeholder="Số giấy chứng nhận" />
			</div>

			<div class="form-row-2">
				<div class="form-group">
					<label>Nhà tự mua, tự xây (loại nhà)</label>
					<input type="text" placeholder="Loại nhà" />
				</div>
				<div class="form-group">
					<label>Tổng diện tích sử dụng (m²)</label>
					<input type="number" placeholder="m²" />
				</div>
			</div>
			<div class="form-group">
				<label>Giấy chứng nhận quyền sở hữu</label>
				<input type="text" placeholder="Số giấy chứng nhận" />
			</div>

			<h5 style="margin: 20px 0 15px; color: #666;">Đất ở</h5>
			<div class="form-row-2">
				<div class="form-group">
					<label>Đất được cấp (m²)</label>
					<input type="number" placeholder="m²" />
				</div>
				<div class="form-group">
					<label>Giấy chứng nhận quyền sử dụng</label>
					<input type="text" placeholder="Số giấy chứng nhận" />
				</div>
			</div>

			<div class="form-row-2">
				<div class="form-group">
					<label>Đất tự mua (m²)</label>
					<input type="number" placeholder="m²" />
				</div>
				<div class="form-group">
					<label>Giấy chứng nhận quyền sử dụng</label>
					<input type="text" placeholder="Số giấy chứng nhận" />
				</div>
			</div>

			<div class="form-group">
				<label>Đất sản xuất kinh doanh</label>
				<textarea placeholder="Mô tả chi tiết về đất sản xuất kinh doanh"></textarea>
			</div>
		</div>
	</div>

	<!-- Section 12: Nhận xét đánh giá -->
	<div class="card">
		<div class="card-header">
			<h2 class="card-title">
				<span>📝</span>
				<span>38) NHẬN XÉT, ĐÁNH GIÁ CỦA CƠ QUAN, TỔ CHỨC, ĐƠN VỊ SỬ DỤNG</span>
			</h2>
		</div>

		<div class="form-section">
			<div class="form-group">
				<label>Nhận xét, đánh giá</label>
				<textarea rows="5" placeholder="Nhận xét, đánh giá của cơ quan..."></textarea>
			</div>
		</div>
	</div>

	<!-- Action Buttons -->
	<div class="card">
		<div class="btn-group">
			<button class="btn btn-outline">
				<span>💾</span>
				<span>Lưu nháp</span>
			</button>
			<button class="btn btn-success">
				<span>✓</span>
				<span>Xác nhận thông tin</span>
			</button>
			<button class="btn btn-primary">
				<span>📤</span>
				<span>Gửi Sơ yếu lý lịch</span>
			</button>
			<button class="btn btn-outline">
				<span>📄</span>
				<span>Xuất file Word</span>
			</button>
			<button class="btn btn-outline">
				<span>🖨️</span>
				<span>In Sơ yếu lý lịch</span>
			</button>
		</div>
	</div>
</div>

<style>
	* {
		margin: 0;
		padding: 0;
		box-sizing: border-box;
	}

	body {
		font-family: 'Times New Roman', Times, serif;
		background-color: #f5f5f5;
		color: #333;
		line-height: 1.6;
	}

	.container {
		max-width: 1200px;
		margin: 0 auto;
		padding: 20px;
	}

	/* Header Styles */
	.header {
		background: linear-gradient(135deg, #0b57d0 0%, #073e9d 100%);
		color: white;
		padding: 20px 0;
		box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
		margin-bottom: 30px;
	}

	.header-content {
		display: flex;
		justify-content: space-between;
		align-items: center;
		max-width: 1200px;
		margin: 0 auto;
		padding: 0 20px;
	}

	.logo {
		font-size: 20px;
		font-weight: bold;
	}

	.user-info {
		display: flex;
		align-items: center;
		gap: 20px;
	}

	/* Form Header */
	.form-header {
		background: white;
		padding: 30px;
		border-radius: 12px;
		margin-bottom: 20px;
		box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
	}

	.form-title {
		text-align: center;
		margin-bottom: 20px;
	}

	.form-title h1 {
		color: #0b57d0;
		font-size: 24px;
		margin-bottom: 10px;
	}

	.form-meta {
		display: grid;
		grid-template-columns: repeat(3, 1fr);
		gap: 20px;
		margin-bottom: 20px;
	}

	.photo-section {
		display: flex;
		gap: 30px;
		align-items: flex-start;
		margin-bottom: 20px;
	}

	.photo-upload {
		width: 150px;
		height: 200px;
		border: 2px dashed #ddd;
		display: flex;
		align-items: center;
		justify-content: center;
		background: #f9f9f9;
		cursor: pointer;
		position: relative;
		overflow: hidden;
	}

	.photo-upload:hover {
		border-color: #0b57d0;
		background: #e8f0fe;
	}

	.photo-upload img {
		width: 100%;
		height: 100%;
		object-fit: cover;
	}

	.basic-info {
		flex: 1;
	}

	/* Progress Tracker */
	.progress-tracker {
		display: flex;
		justify-content: space-between;
		margin-bottom: 30px;
		background: white;
		padding: 20px;
		border-radius: 12px;
		box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
	}

	.progress-step {
		text-align: center;
		flex: 1;
		position: relative;
	}

	.progress-step::after {
		content: '';
		position: absolute;
		top: 20px;
		right: -50%;
		width: 100%;
		height: 2px;
		background: #e0e0e0;
		z-index: -1;
	}

	.progress-step:last-child::after {
		display: none;
	}

	.progress-circle {
		width: 40px;
		height: 40px;
		margin: 0 auto 10px;
		background: #e0e0e0;
		border-radius: 50%;
		display: flex;
		align-items: center;
		justify-content: center;
		font-weight: bold;
		color: #999;
	}

	.progress-step.active .progress-circle {
		background: #0b57d0;
		color: white;
	}

	.progress-step.completed .progress-circle {
		background: #28a745;
		color: white;
	}

	.progress-label {
		font-size: 12px;
		color: #666;
	}

	/* Card Styles */
	.card {
		background: white;
		border-radius: 12px;
		padding: 30px;
		box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
		margin-bottom: 20px;
	}

	.card-header {
		border-bottom: 2px solid #e0e0e0;
		padding-bottom: 15px;
		margin-bottom: 25px;
	}

	.card-title {
		font-size: 18px;
		font-weight: 600;
		color: #0b57d0;
		display: flex;
		align-items: center;
		gap: 10px;
	}

	/* Form Styles */
	.form-section {
		margin-bottom: 30px;
	}

	.form-row {
		display: grid;
		grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
		gap: 20px;
		margin-bottom: 20px;
	}

	.form-row-2 {
		display: grid;
		grid-template-columns: repeat(2, 1fr);
		gap: 20px;
		margin-bottom: 20px;
	}

	.form-row-3 {
		display: grid;
		grid-template-columns: repeat(3, 1fr);
		gap: 20px;
		margin-bottom: 20px;
	}

	.form-row-4 {
		display: grid;
		grid-template-columns: repeat(4, 1fr);
		gap: 20px;
		margin-bottom: 20px;
	}

	.form-group {
		display: flex;
		flex-direction: column;
	}

	.form-group-inline {
		display: flex;
		gap: 10px;
		align-items: center;
	}

	label {
		font-weight: 500;
		margin-bottom: 8px;
		color: #555;
		font-size: 14px;
	}

	.required {
		color: red;
	}

	input[type='text'],
	input[type='email'],
	input[type='tel'],
	input[type='date'],
	input[type='number'],
	select,
	textarea {
		padding: 10px;
		border: 1px solid #ddd;
		border-radius: 6px;
		font-size: 14px;
		transition: all 0.3s;
		font-family: 'Times New Roman', Times, serif;
	}

	input:focus,
	select:focus,
	textarea:focus {
		outline: none;
		border-color: #0b57d0;
		box-shadow: 0 0 0 3px rgba(11, 87, 208, 0.1);
	}

	textarea {
		min-height: 80px;
		resize: vertical;
	}

	.synced-field {
		background: #e8f0fe !important;
		position: relative;
	}

	/* Table Styles */
	.dynamic-table {
		overflow-x: auto;
		margin: 20px 0;
	}

	table {
		width: 100%;
		border-collapse: collapse;
		font-size: 14px;
	}

	th {
		background: #f5f5f5;
		padding: 10px;
		text-align: left;
		font-weight: 600;
		color: #555;
		border: 1px solid #ddd;
	}

	td {
		padding: 8px;
		border: 1px solid #ddd;
	}

	td input,
	td select {
		width: 100%;
		padding: 6px;
		border: 1px solid #ddd;
		border-radius: 4px;
	}

	.add-row-btn {
		display: inline-flex;
		align-items: center;
		gap: 8px;
		padding: 10px 20px;
		background: #0b57d0;
		color: white;
		border: none;
		border-radius: 6px;
		cursor: pointer;
		font-size: 14px;
		transition: all 0.3s;
	}

	.add-row-btn:hover {
		background: #073e9d;
	}

	.remove-btn {
		background: #dc3545;
		color: white;
		border: none;
		padding: 5px 10px;
		border-radius: 4px;
		cursor: pointer;
	}

	/* Button Styles */
	.btn {
		padding: 12px 24px;
		border: none;
		border-radius: 8px;
		font-size: 14px;
		font-weight: 500;
		cursor: pointer;
		transition: all 0.3s;
		display: inline-flex;
		align-items: center;
		gap: 8px;
	}

	.btn-primary {
		background: #0b57d0;
		color: white;
	}

	.btn-primary:hover {
		background: #073e9d;
	}

	.btn-success {
		background: #28a745;
		color: white;
	}

	.btn-outline {
		background: white;
		border: 2px solid #0b57d0;
		color: #0b57d0;
	}

	.btn-group {
		display: flex;
		gap: 15px;
		margin-top: 30px;
		padding-top: 30px;
		border-top: 2px solid #e0e0e0;
	}

	/* Alert */
	.alert {
		padding: 15px 20px;
		border-radius: 8px;
		margin-bottom: 20px;
		display: flex;
		align-items: center;
		gap: 10px;
	}

	.alert-info {
		background: #cfe2ff;
		color: #084298;
		border-left: 4px solid #084298;
	}

	/* Responsive */
	@media (max-width: 768px) {
		.form-row,
		.form-row-2,
		.form-row-3,
		.form-row-4 {
			grid-template-columns: 1fr;
		}

		.photo-section {
			flex-direction: column;
		}

		.btn-group {
			flex-direction: column;
		}

		.btn {
			width: 100%;
			justify-content: center;
		}
	}

	@media print {
		.header,
		.progress-tracker,
		.btn-group,
		.alert,
		.add-row-btn,
		.remove-btn {
			display: none !important;
		}

		body {
			background: white;
		}

		.card {
			box-shadow: none;
			page-break-inside: avoid;
		}
	}
</style>
