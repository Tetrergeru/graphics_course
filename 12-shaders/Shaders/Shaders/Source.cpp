#include <stdio.h>;
#include <gl/glew.h>
#include <GL/glut.h>;
#include <GL/freeglut.h>;
#include <GL/freeglut_ext.h>
#include <vector>;
#include <functional>
#include <conio.h>
#include <iostream>
//--------------------------------начало Shader--------------------------------
//! ID шейдерной программы
GLuint Program;
//! ID атрибута
GLint Attrib_vertex;
//! ID юниформ переменной цвета
GLint Unif_color;

GLint Unif_angle;

//! Проверка ошибок OpenGL, если есть то вывод в консоль тип ошибки
void checkOpenGLerror() 
{
	GLenum errCode;
	if ((errCode = glGetError()) != GL_NO_ERROR)
		std::cout << "OpenGl error! - " << gluErrorString(errCode);
}
//! Инициализация шейдеров
void initShaderStrips()
{
	//! Исходный код шейдеров
	const char* vsSource =
		"attribute vec2 coord;\n"
		"void main() {\n"
		" gl_Position = vec4(coord, 0.0, 1.0);\n"
		"}\n";
	const char* fsSource =
		"uniform vec4 color;\n"
		"void main() {\n"
		" if (mod(gl_FragCoord.x, 30)<15.0) \n"
		" gl_FragColor = color;\n"
		" else\n"
		" gl_FragColor = vec4(1.0,1.0,1.0,0.0);\n"
		"}\n";

	//! Переменные для хранения идентификаторов шейдеров
	GLuint fShader, vShader;
	//! Создаем вершинный шейдер
	vShader = glCreateShader(GL_VERTEX_SHADER);
	//! Передаем исходный код
	glShaderSource(vShader, 1, &vsSource, NULL);
	//! Компилируем шейдер
	glCompileShader(vShader);
	//! Создаем фрагментный шейдер
	fShader = glCreateShader(GL_FRAGMENT_SHADER);
	//! Передаем исходный код
	glShaderSource(fShader, 1, &fsSource, NULL);
	//! Компилируем шейдер
	glCompileShader(fShader);
	//! Создаем программу и прикрепляем шейдеры к ней
	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glAttachShader(Program, fShader);
	//! Линкуем шейдерную программу
	glLinkProgram(Program);
	//! Проверяем статус сборки
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}
	///! Вытягиваем ID атрибута из собранной программы
	const char* attr_name = "coord";
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1)
	{
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}
	//! Вытягиваем ID юниформ
	const char* unif_name = "color";
	Unif_color = glGetUniformLocation(Program, unif_name);
	if (Unif_color == -1)
	{
		std::cout << "could not bind uniform " << unif_name << std::endl;
		return;
	}
	checkOpenGLerror();
}

void initShaderSquares()
{
	//! Исходный код шейдеров
	const char* vsSource =
		"attribute vec2 coord;\n"
		"void main() {\n"
		" gl_Position = vec4(coord, 0.0, 1.0);\n"
		"}\n";
	const char* fsSource =
		"uniform vec4 color;\n"
		"void main() {\n"
		" if ((mod(gl_FragCoord.x, 30)<15.0 && mod(gl_FragCoord.y, 30)>15.0) || (mod(gl_FragCoord.x, 30)>15.0 && mod(gl_FragCoord.y, 30)<15.0)) \n"
		" gl_FragColor = color;\n"
		" else\n"
		" gl_FragColor = vec4(1.0,1.0,1.0,0.0);\n"
		"}\n";

	//! Переменные для хранения идентификаторов шейдеров
	GLuint fShader, vShader;
	//! Создаем вершинный шейдер
	vShader = glCreateShader(GL_VERTEX_SHADER);
	//! Передаем исходный код
	glShaderSource(vShader, 1, &vsSource, NULL);
	//! Компилируем шейдер
	glCompileShader(vShader);
	//! Создаем фрагментный шейдер
	fShader = glCreateShader(GL_FRAGMENT_SHADER);
	//! Передаем исходный код
	glShaderSource(fShader, 1, &fsSource, NULL);
	//! Компилируем шейдер
	glCompileShader(fShader);
	//! Создаем программу и прикрепляем шейдеры к ней
	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glAttachShader(Program, fShader);
	//! Линкуем шейдерную программу
	glLinkProgram(Program);
	//! Проверяем статус сборки
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}
	///! Вытягиваем ID атрибута из собранной программы
	const char* attr_name = "coord";
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1)
	{
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}
	//! Вытягиваем ID юниформ
	const char* unif_name = "color";
	Unif_color = glGetUniformLocation(Program, unif_name);
	if (Unif_color == -1)
	{
		std::cout << "could not bind uniform " << unif_name << std::endl;
		return;
	}
	checkOpenGLerror();
}

void initShaderTriangle()
{
	const char* vsSource =
		"attribute vec2 coord;\n"
		"uniform float angle;\n"
		"varying vec4 var_color; \n"
		"mat2 rot(in float a) {return mat2(cos(a), sin(a), -sin(a), cos(a));}\n"
		"void main() {\n"
		" vec2 pos = rot(3.14*angle)*coord;\n"
		" gl_Position = vec4(pos, 0, 1.0);\n"
		" var_color = gl_Color;\n"
		"}\n";

	const char* fsSource =
		"varying vec4 var_color;\n"
		"void main() {\n"
		" gl_FragColor = var_color;\n"
		"}\n";
	GLuint vShader, fShader;
	vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vsSource, NULL);
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &fsSource, NULL);
	glCompileShader(fShader);
	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glAttachShader(Program, fShader);

	glLinkProgram(Program);
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok) 
	{
		std::cout << "error attach shaders \n"; 
		return;
	}
	const char* attr_name = "coord";
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1)
	{
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}
	Unif_angle =  glGetUniformLocation(Program, "angle");
	if (Unif_angle == -1)
	{
		std::cout << "could not bind uniform " << "angle" << std::endl;
	}
	checkOpenGLerror();
}

void initShaderCube2()
{
	const char* vsSource =
		"attribute vec3 coord;\n"
		"uniform float angle;\n"
		"mat3 rot(in float a) {return mat3(1.0, 0.0, 0.0, 0.0, cos(a), -sin(a), 0.0, sin(a), cos(a)) *\n"
		"                             mat3(cos(a), 0.0, sin(a), 0.0, 1.0, 0.0, -sin(a), 0.0, cos(a))  ;}\n"
		"void main() {\n"
		" vec3 pos = rot(3.14*1.3)*coord;\n"
		" gl_Position = vec4(pos, 1.0);\n"
		" gl_FrontColor = vec4(pos + vec3(0.5, 0.5, 0.5), 1.0);\n"
		"}\n";

	const char* fsSource =
		"void main() {\n"
		" gl_FragColor = gl_Color;\n"
		"}\n";
	GLuint vShader, fShader;
	vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vsSource, NULL);
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &fsSource, NULL);
	glCompileShader(fShader);
	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glAttachShader(Program, fShader);

	glLinkProgram(Program);
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}
	const char* attr_name = "coord";
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1)
	{
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}
	Unif_angle = glGetUniformLocation(Program, "angle");
	if (Unif_angle == -1)
	{
		std::cout << "could not bind uniform " << "angle" << std::endl;
	}
	checkOpenGLerror();
}

void initShaderCube()
{
	const char* vsSource =
		"attribute vec2 coord;\n"
		"void main() {\n"
		"  gl_Position = vec4(coord, 0.0, 1.0);\n"
		"}\n";

	const char* fsSource =
		"uniform vec4 color;\n"
		"void main() {\n"
		" if (mod(gl_FragCoord.x, 30)<15.0) \n"
		" gl_FragColor = color;\n"
		" else\n"
		" gl_FragColor = vec4(1.0,1.0,1.0,0.0);\n"
		"}\n";
	GLuint vShader, fShader;
	vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vsSource, NULL);
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &fsSource, NULL);
	glCompileShader(fShader);
	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glAttachShader(Program, fShader);

	glLinkProgram(Program);
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}
	const char* attr_name = "coord";
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1)
	{
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}
	const char* unif_name = "color";
	Unif_color = glGetUniformLocation(Program, unif_name);
	if (Unif_color == -1)
	{
		std::cout << "could not bind uniform " << unif_name << std::endl;
		return;
	}
	checkOpenGLerror();
}

//! Освобождение шейдеров
void freeShader()
{
	//! Передавая ноль, мы отключаем шейдрную программу
	glUseProgram(0);
	//! Удаляем шейдерную программу
	glDeleteProgram(Program);
}
//--------------------------------конец Shader--------------------------------

static int w = 0, h = 0;
float Angle = 0;
std::vector<std::function<void()>> vFunc;
int current = 0;

void Init(void)
{
	glClearColor(0.0f, 0.0f, 1.0f, 1.0f);
}

void RenderRectangle()
{
	glMatrixMode(GL_MODELVIEW);
	Angle += 0.05f;
	glClear(GL_COLOR_BUFFER_BIT);
	glLoadIdentity();
	gluLookAt(0.0f, 0.0f, 2.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
	glRotatef(Angle, 0.0f, 0.0f, 1.0f);

	//! Устанавливаем шейдерную программу текущей
	glUseProgram(Program);
	static float red[4] = { 1.0f, 0.0f, 0.0f, 1.0f };
	//! Передаем юниформ в шейдер
	glUniform4fv(Unif_color, 1, red);

	glBegin(GL_QUADS);
	glVertex3f(-0.5f, -0.5f, 0);
	glVertex3f(-0.5f, 0.5f, 0);
	glVertex3f(0.5f, 0.5f, 0);
	glVertex3f(0.5f, -0.5f, 0);
	glEnd();
	glFlush(); 

	//! Отключаем шейдерную программу
	glUseProgram(0); 
	
	glutSwapBuffers();
}

void RenderTriangle()
{
	glMatrixMode(GL_MODELVIEW);
	Angle += 0.01f;
	glClear(GL_COLOR_BUFFER_BIT);
	glLoadIdentity();
	//gluLookAt(100.0f, 100.0f, 100.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
	glRotatef(Angle, 0.0f, 1.0f, 0.0f);
	//! Устанавливаем шейдерную программу текущей
	glUseProgram(Program);
	//static float red[4] = { 1.0f, 0.0f, 0.0f, 1.0f };
	//! Передаем юниформ в шейдер
	glUniform1f(Unif_angle, Angle);

	glBegin(GL_TRIANGLES);
	glColor3f(0, 1, 1);  glVertex2f(-0.5f, -0.5f);
	glColor3f(1, 0.2, 0);  glVertex2f(-0.5f, 0.5f);
	glColor3f(1, 0, 1);  glVertex2f(0.5f, 0.5f);
	glEnd();
	glutSwapBuffers();
}

void RenderSolidCube()
{
	glMatrixMode(GL_MODELVIEW);
	Angle += 0.01f;
	glClear(GL_COLOR_BUFFER_BIT);
	glLoadIdentity();
	gluLookAt(100.0f, 100.0f, 100.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
	//glRotatef(Angle, 0.0f, 1.0f, 0.0f);
	//! Устанавливаем шейдерную программу текущей
	glUseProgram(Program);
	//static float red[4] = { 1.0f, 0.0f, 0.0f, 1.0f };
	//! Передаем юниформ в шейдер
	glUniform1f(Unif_angle, Angle);

	glutSolidCube(1.0);

	glutSwapBuffers();
}

void InitFuncVector()
{
	//vFunc.push_back(RenderRectangle);
	//vFunc.push_back(RenderTriangle);
	vFunc.push_back(RenderSolidCube);
}

void Update(void)
{
	if (_kbhit() == 1)
	{
		if (current < (vFunc.size() - 1))
			current += 1;
		else
			current = 0;
		char k = _getch();
	}
	vFunc[current]();
}

void Reshape(int width, int height)
{
	w = width;
	h = height;
	glViewport(0, 0, w, h);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	gluPerspective(65.0f, w / h, 1.0f, 1000.0f);
}

int main(int argc, char* argv[])
{
	InitFuncVector();
	glutInit(&argc, argv);
	glutInitWindowPosition(100, 100);
	glutInitWindowSize(600, 600);
	glEnable(GL_DEPTH_TEST);
	glDepthFunc(GL_LESS);
	glutInitDisplayMode(GLUT_DEPTH | GLUT_DOUBLE | GLUT_RGBA);
	glutCreateWindow("OpenGL");

	GLenum glew_status = glewInit();
	if (GLEW_OK != glew_status)
	{
		std::cout << "Error: " << glewGetErrorString(glew_status) << "\n";
		return 1;
	}
	if (!GLEW_VERSION_2_0)
	{
		std::cout << "No support for OpenGL 2.0 found\n";
		return 1;
	}

	//initShaderStrips();
	//initShaderSquares();
	//initShaderTriangle();
	//initShaderCube();
	initShaderCube2();

	glutIdleFunc(Update);
	glutDisplayFunc(Update);
	glutReshapeFunc(Reshape);
	glutMainLoop();

	freeShader();
	return 0;
}