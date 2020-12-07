#include <stdio.h>;
#include <gl/glew.h>
#include <GL/glut.h>;
#include <GL/freeglut.h>;
#include <GL/freeglut_ext.h>
#include <vector>;
#include <functional>
#include <conio.h>
#include <iostream>

//--------------------------------������ Shader--------------------------------
//! ID ��������� ���������
GLuint Program;
//! ID ��������
GLint Attrib_vertex;
//! ID ������� ���������� �����
GLint Unif_color;
//! �������� ������ OpenGL, ���� ���� �� ����� � ������� ��� ������
void checkOpenGLerror() 
{
	GLenum errCode;
	if ((errCode = glGetError()) != GL_NO_ERROR)
		std::cout << "OpenGl error! - " << gluErrorString(errCode);
}
//! ������������� ��������
void initShaderStrips()
{
	//! �������� ��� ��������
	const char* vsSource =
		"attribute vec2 coord;\n"
		"void main() {\n"
		" gl_Position = vec4(coord, 0.0, 1.0);\n"
		"}\n";
	const char* fsSource =
		"uniform vec4 color;\n"
		"void main() {\n"
		" if (mod(gl_FragCoord.x, 10)<5.0) \n"
		" gl_FragColor = color;\n"
		" else\n"
		" gl_FragColor = vec4(1.0,1.0,1.0,0.0);\n"
		"}\n";

	//! ���������� ��� �������� ��������������� ��������
	GLuint fShader, vShader;
	//! ������� ��������� ������
	vShader = glCreateShader(GL_VERTEX_SHADER);
	//! �������� �������� ���
	glShaderSource(vShader, 1, &vsSource, NULL);
	//! ����������� ������
	glCompileShader(vShader);
	//! ������� ����������� ������
	fShader = glCreateShader(GL_FRAGMENT_SHADER);
	//! �������� �������� ���
	glShaderSource(fShader, 1, &fsSource, NULL);
	//! ����������� ������
	glCompileShader(fShader);
	//! ������� ��������� � ����������� ������� � ���
	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glAttachShader(Program, fShader);
	//! ������� ��������� ���������
	glLinkProgram(Program);
	//! ��������� ������ ������
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}
	///! ���������� ID �������� �� ��������� ���������
	const char* attr_name = "coord";
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1)
	{
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}
	//! ���������� ID �������
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
	//! �������� ��� ��������
	const char* vsSource =
		"attribute vec2 coord;\n"
		"void main() {\n"
		" gl_Position = vec4(coord, 0.0, 1.0);\n"
		"}\n";
	const char* fsSource =
		"uniform vec4 color;\n"
		"void main() {\n"
		" if (mod(gl_FragCoord.x, 10)<5.0 && mod(gl_FragCoord.y, 10)>5.0) \n"
		" gl_FragColor = color;\n"
		" else\n"
		" gl_FragColor = vec4(1.0,0.0,1.0,0.0);\n"
		"}\n";

	//! ���������� ��� �������� ��������������� ��������
	GLuint fShader, vShader;
	//! ������� ��������� ������
	vShader = glCreateShader(GL_VERTEX_SHADER);
	//! �������� �������� ���
	glShaderSource(vShader, 1, &vsSource, NULL);
	//! ����������� ������
	glCompileShader(vShader);
	//! ������� ����������� ������
	fShader = glCreateShader(GL_FRAGMENT_SHADER);
	//! �������� �������� ���
	glShaderSource(fShader, 1, &fsSource, NULL);
	//! ����������� ������
	glCompileShader(fShader);
	//! ������� ��������� � ����������� ������� � ���
	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glAttachShader(Program, fShader);
	//! ������� ��������� ���������
	glLinkProgram(Program);
	//! ��������� ������ ������
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}
	///! ���������� ID �������� �� ��������� ���������
	const char* attr_name = "coord";
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1)
	{
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}
	//! ���������� ID �������
	const char* unif_name = "color";
	Unif_color = glGetUniformLocation(Program, unif_name);
	if (Unif_color == -1)
	{
		std::cout << "could not bind uniform " << unif_name << std::endl;
		return;
	}
	checkOpenGLerror();
}

//! ������������ ��������
void freeShader()
{
	//! ��������� ����, �� ��������� �������� ���������
	glUseProgram(0);
	//! ������� ��������� ���������
	glDeleteProgram(Program);
}
//--------------------------------����� Shader--------------------------------

static int w = 0, h = 0;
double rotate_x = 0;
double rotate_y = 0;
double rotate_z = 0;
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

	//! ������������� ��������� ��������� �������
	glUseProgram(Program);
	static float red[4] = { 1.0f, 0.0f, 0.0f, 1.0f };
	//! �������� ������� � ������
	glUniform4fv(Unif_color, 1, red);
	glBegin(GL_QUADS);
	glVertex3f(-0.5f, -0.5f, 0);
	glVertex3f(-0.5f, 0.5f, 0);
	glVertex3f(0.5f, 0.5f, 0);
	glVertex3f(0.5f, -0.5f, 0);
	glEnd();
	glFlush(); 

	//! ��������� ��������� ���������
	glUseProgram(0); 
	
	glutSwapBuffers();
}

void RenderTriangle()
{
	glMatrixMode(GL_MODELVIEW);
	Angle += 0.05f;
	glClear(GL_COLOR_BUFFER_BIT);
	glLoadIdentity();
	gluLookAt(100.0f, 100.0f, 100.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
	glRotatef(Angle, 0.0f, 1.0f, 0.0f);
	glBegin(GL_TRIANGLES);
	glColor3ub(0, 0, 255);
	glVertex3f(0.0, 0.0, 0.0);
	glColor3ub(255, 0, 0);
	glVertex3f(75.0, 0.0, 0.0);
	glColor3ub(0, 255, 0);
	glVertex3f(75.0, 75.0, 0.0);
	glEnd();
	glutSwapBuffers();
	glPointSize(10.0f);
}

void RenderSolidCube()
{
	glMatrixMode(GL_MODELVIEW);
	Angle += 0.05f;
	glClear(GL_COLOR_BUFFER_BIT);
	glLoadIdentity();
	gluLookAt(100.0f, 100.0f, 100.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
	glRotatef(Angle, 0.0f, 1.0f, 0.0f);
	glutSolidCube(70);
	glColor3ub(255, 0, 0);
	glutWireCube(71);
	glColor3ub(255, 255, 255);
	glutSwapBuffers();

}

void InitFuncVector()
{
	vFunc.push_back(RenderRectangle);
	vFunc.push_back(RenderSolidCube);
	vFunc.push_back(RenderTriangle);
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

	initShaderStrips();
	//initShaderSquares();

	glutIdleFunc(Update);
	glutDisplayFunc(Update);
	glutReshapeFunc(Reshape);
	glutMainLoop();

	freeShader();
	return 0;
}