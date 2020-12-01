//#include "GL/soil.h"
#define _USE_MATH_DEFINES
#include "GL/glew.h"
#include "GL/freeglut.h"
#include <iostream>
#include <math.h>
static int w = 0, h = 0;

GLfloat distX = 0, distY = 0;
GLfloat angle = 0;

GLfloat cam_dist = 20;
GLfloat ang_hor = 0, ang_vert = -60;

double camX = 0;
double camY = 0;
double camZ = 0;


void init() {
    glClearColor(0, 0, 0, 1);

    glutInitDisplayMode(GLUT_RGBA | GLUT_DOUBLE | GLUT_DEPTH);

    glEnable(GL_DEPTH_TEST);
    glEnable(GL_COLOR_MATERIAL);
	glEnable(GL_LIGHTING);
	glEnable(GL_LIGHT0);
}

void drawFloor() 
{
    glBegin(GL_QUADS);
    glTexCoord2f(0, 0); glNormal3f(0, 0, 1); glVertex3f(-10, -10, 0);
    glTexCoord2f(0, 1); glNormal3f(0, 0, 1); glVertex3f(-10, 10, 0);
    glTexCoord2f(1, 1); glNormal3f(0, 0, 1); glVertex3f(10, 10, 0);
    glTexCoord2f(1, 0); glNormal3f(0, 0, 1); glVertex3f(10, -10, 0);
    glEnd();
}


void drawLamp(GLfloat x, GLfloat y) 
{
    glPushMatrix();
    glTranslatef(x, y, 0);
    glutSolidCylinder(0.1, 4, 8, 8);
    glPushMatrix();
    glTranslatef(0, 0, 4.1);
    glutSolidSphere(0.6, 8, 8);
    glPopMatrix();
    glPopMatrix();
}

void drawCar(GLfloat x = 5, GLfloat y = 5)
{
	glPushMatrix();
	glTranslated(x, y, 0);

	glPushMatrix();
	glTranslated(distX, distY, 1);
	glRotated(angle, 0, 0, 1);
	//Кузов
	glPushMatrix();
	glScaled(2, 1, 1);
	glutSolidCube(1);
	glPopMatrix();
	//Задние колеса
	glPushMatrix();
	glTranslated(-0.5, -0.5, -0.6);
	glRotated(90, 1, 0, 0);
	glutSolidTorus(0.15, 0.2, 20, 20);
	glTranslated(0, 0, -1);
	glutSolidTorus(0.15, 0.2, 20, 20);
	glPopMatrix();
	//Кабина
	glPushMatrix();
	glTranslated(1.3, 0, -0.1);
	glutSolidCube(0.6);
	//Фары
	glPushMatrix();
	glTranslated(0.4, 0.3, 0);
	glutSolidSphere(0.05,20,20);
	glTranslated(0, -0.6, 0);
	glutSolidSphere(0.05,20,20);
	glPopMatrix();
	//Передние колеса
	glTranslated(-0.6, -0.5, -0.5);
	glRotated(90, 1, 0, 0);
	glutSolidTorus(0.15,0.2,20,20);
	glTranslated(0, 0,-1);
	glutSolidTorus(0.15, 0.2, 20, 20);
	glPopMatrix();

	glPopMatrix();
	glPopMatrix();
    glPopMatrix();
}

void drawCristmasTree()
{
    //ствол
    glutSolidCylinder(0.8, 5, 6, 2);
    
    //конусы(иголочки)
	glPushMatrix();
	glTranslated(0, 0, 1.5);
	GLUquadricObj* bot = gluNewQuadric();
	gluQuadricDrawStyle(bot, GLU_FILL);
	gluCylinder(bot, 3, 0.1, 4, 16, 16);
    glPopMatrix();

	glPushMatrix();
	glTranslated(0, 0, 3);
	GLUquadricObj* middle = gluNewQuadric();
	gluQuadricDrawStyle(middle, GLU_FILL);
	gluCylinder(middle, 2, 0.1, 4, 16, 16);
	glPopMatrix();

	glPushMatrix();
	glTranslated(0, 0, 4.2);
	GLUquadricObj* top = gluNewQuadric();
	gluQuadricDrawStyle(top, GLU_FILL);
	gluCylinder(top, 1.5, 0.1, 4, 16, 16);
	glPopMatrix();
    //шарики
	glPushMatrix();
	glTranslated(1.65, 1, 3.2);
    glutSolidSphere(0.3, 8, 8);

	glTranslated(-3, -0.77, 2);
	glutSolidSphere(0.3, 8, 8);

	glTranslated(1.8, -1.8, -1);
	glutSolidSphere(0.3, 8, 8);

    glPopMatrix();

    //герлянды
	glPushMatrix();
    double startX = 2.57;
    double startY = 1.12;
	glTranslated(2.45, 1, 2.2);
    for (int i = 0; i < 24; i++)
    {
        glTranslated(0.15, 0.15, 0); //!FIXME вращение по окружности
        glutSolidSphere(0.1, 6, 6);
    }
    glPopMatrix();
}

void update() {
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    glLoadIdentity();

    double ang_vert_r = ang_vert / 180 * M_PI;
    double ang_hor_r = ang_hor / 180 * M_PI;
    camX = cam_dist * std::sin(ang_vert_r) * std::cos(ang_hor_r);
    camY = cam_dist * std::sin(ang_vert_r) * std::sin(ang_hor_r);
    camZ = cam_dist * std::cos(ang_vert_r);

    gluLookAt(camX, camY, camZ, 0., 0., 0., 0., 0., 1.);

    drawLamp(7, -7);
    drawLamp(-7, 7);
    drawLamp(7, 7);
    drawLamp(-7, -7);

    drawFloor();

    drawCristmasTree();

	drawCar();

    glFlush();
    glutSwapBuffers();
}

void updateCamera() {
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluPerspective(60.f, (float)w / h, 1.0f, 1000.f);
    glMatrixMode(GL_MODELVIEW);
}


void keyboard(unsigned char key, int x, int y) {
    switch (key) {
    case 'w':
        ang_vert += 5;
        break;
    case 's':
        ang_vert -= 5;
        break;
    case 'a':
        ang_hor -= 5;
        break;
    case 'd':
        ang_hor += 5;
        break;
    }
    glutPostRedisplay();
}

void reshape(int width, int height) {
    w = width;
    h = height;

    glViewport(0, 0, w, h);
    updateCamera();
}


int main(int argc, char* argv[]) {
    glutInit(&argc, argv);
    glutInitWindowPosition(100, 100);
    glutInitWindowSize(800, 800);
    glutCreateWindow("texture and lighting");

    init();

    glutReshapeFunc(reshape);
    glutDisplayFunc(update);
    glutKeyboardFunc(keyboard);

    glutMainLoop();

    return 0;
}
