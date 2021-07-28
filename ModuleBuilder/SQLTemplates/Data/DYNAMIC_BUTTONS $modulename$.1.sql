
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.DetailView', '$modulename$.DetailView', '$modulename$';
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.EditView'  , '$modulename$.EditView'  , '$modulename$';
exec dbo.spDYNAMIC_BUTTONS_CopyDefault '.PopupView' , '$modulename$.PopupView' , '$modulename$';
GO

