const imageInput = document.getElementById('imageInput')

imageInput.addEventListener('change', updateImage);

function updateImage() {
	const file = this.files[0];
	const reader = new FileReader();
	reader.onload = function (event) {
		document.getElementById('contactImage').src = event.target.result;
	};
	reader.readAsDataURL(file);
}
